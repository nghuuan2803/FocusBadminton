using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Web.CronJobs
{
    public class AutoCancelExpiredBooking : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);
        public AutoCancelExpiredBooking(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CancelExpiredBookingsAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }
        private async Task CancelExpiredBookingsAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var now = DateTimeOffset.UtcNow;
                // Tìm các Booking đã hết hạn
                var expiredBookings = await dbContext.Bookings
                    .Where(b => b.Status == BookingStatus.Creating)
                    //.Where(b => (now - b.CreatedAt).TotalMinutes >= 15 && b.Status == BookingStatus.Creating)
                    .ToListAsync(cancellationToken);
                if (expiredBookings.Count != 0)
                {
                    // Cập nhật trạng thái của các Booking đã hết hạn
                    foreach (var booking in expiredBookings)
                    {
                        if ((now - booking.CreatedAt).TotalMinutes >= 15)

                            // Ghi chú: Có thể thêm logic để gửi thông báo cho người dùng nếu cần
                            booking.Status = BookingStatus.Canceled;
                    }
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
