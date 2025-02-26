using Application.Features.HoldSlots;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Web.Hubs
{
    public class SlotHub : Hub
    {
        private readonly AppDbContext _context;

        public SlotHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task HoldSlot(HoldSlotCommand request)
        {
            var hold = new BookingHold
            {
                CourtId = request.CourtId,
                TimeSlotId = request.TimeSlotId,
                HeldBy = request.HoldBy,
                HeldAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(5),
                BookingType = request.BookingType,
                BeginAt = request.BeginAt,
                EndAt = request.EndAt,
                DayOfWeek = request.DayOfWeek
            };

            _context.BookingHolds.Add(hold);
            await _context.SaveChangesAsync();

            // Thông báo cho các client về slot đã được giữ
            await Clients.All.SendAsync("SlotHeld", request.CourtId, request.TimeSlotId, request.HoldBy, hold.Id);
        }

        public async Task ReleaseSlot(int holdSlotId, string heldBy)
        {
            // Tìm record BookingHold theo holdSlotId và heldBy
            var hold = await _context.BookingHolds
                .FirstOrDefaultAsync(b => b.Id == holdSlotId && b.HeldBy == heldBy);

            if (hold != null)
            {
                // Chuẩn bị payload chứa các thông tin chi tiết cần gửi về client,
                // giúp client cập nhật giao diện và xử lý logic tránh overlap lịch đặt.
                var releaseInfo = new
                {
                    HoldSlotId = hold.Id,
                    CourtId = hold.CourtId,
                    TimeSlotId = hold.TimeSlotId,
                    BookingType = (int)hold.BookingType,
                    BeginAt = hold.BeginAt,
                    EndAt = hold.EndAt,
                    DayOfWeek = hold.DayOfWeek
                };

                // Xóa record BookingHold khỏi database
                _context.BookingHolds.Remove(hold);
                await _context.SaveChangesAsync();

                // Thông báo cho tất cả các client rằng slot đã được nhả,
                // gửi kèm payload releaseInfo để client xử lý cập nhật lịch.
                await Clients.All.SendAsync("SlotReleased", releaseInfo);
            }
            else
            {
                // Nếu không tìm thấy record, có thể thông báo lỗi cho client caller
                await Clients.Caller.SendAsync("ReleaseFailed", "Không tìm thấy BookingHold hoặc không có quyền nhả slot.");
            }
        }
    }
}
