using Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.Hubs;

namespace Web.CronJobs
{
    public class AutoReleaseSlot : BackgroundService
    {
        private readonly IHubContext<SlotHub> _hubContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(300);

        public AutoReleaseSlot(IHubContext<SlotHub> hubContext, IServiceScopeFactory serviceScopeFactory)
        {
            _hubContext = hubContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, stoppingToken);
                await CleanUpExpiredHoldsAsync(stoppingToken);
            }
        }

        private async Task CleanUpExpiredHoldsAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var now = DateTimeOffset.UtcNow;

                // Tìm các BookingHold đã hết hạn
                var expiredHolds = await dbContext.BookingHolds
                    .Where(bh => bh.ExpiresAt <= now)
                    .ToListAsync(cancellationToken);

                if (expiredHolds.Count != 0)
                {
                    // Tạo payload chứa các thông tin chi tiết để client cập nhật giao diện tránh overlap lịch đặt
                    var releasedSlots = expiredHolds.Select(bh => new
                    {
                        HoldSlotId = bh.Id,
                        bh.CourtId,
                        bh.TimeSlotId,
                        BookingType = (int)bh.BookingType,
                        bh.BeginAt,
                        bh.EndAt,
                        bh.DayOfWeek,
                        bh.HeldBy
                    }).ToList();

                    // Xóa các record BookingHold hết hạn
                    dbContext.BookingHolds.RemoveRange(expiredHolds);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    // Thông báo cho tất cả client rằng các slot tương ứng đã được nhả, gửi payload chi tiết
                    foreach (var slot in releasedSlots)
                    {
                        await _hubContext.Clients.All.SendAsync("SlotReleased", slot, cancellationToken);
                    }
                }
            }
        }
    }
}
