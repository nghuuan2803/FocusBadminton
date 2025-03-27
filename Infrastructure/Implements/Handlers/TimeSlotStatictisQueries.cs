using Application.Features.Statictis;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Statistic;

namespace Infrastructure.Implements.Handlers
{
    public class TimeSlotStatictisQueries : ITimeSlotStatisticQueries
    {
        private readonly AppDbContext _context; // Thay YourDbContext bằng tên DbContext của bạn

        public TimeSlotStatictisQueries(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TimeSlotStatisticDTO> Excute(TimeSlotStatictisQuery request, CancellationToken cancellationToken)
        {
            DateTimeOffset startDate = request.StartDate ??
             new DateTimeOffset(new DateTime(request.Year, request.Month ?? 1, 1));

            DateTimeOffset endDate = request.EndDate ??
                (request.Month.HasValue
                    ? startDate.AddMonths(1).AddTicks(-1) // Cuối tháng nếu có Month
                    : new DateTimeOffset(new DateTime(request.Year, 12, 31, 23, 59, 59, 0))); // Cuối năm nếu không có Month

            // Truy vấn tất cả TimeSlot và left join với BookingDetails
            var statistics = await (from ts in _context.TimeSlots
                                    where ts.IsApplied && !ts.IsDeleted // Chỉ lấy TimeSlot đang áp dụng
                                    join bd in _context.BookingDetails
                                        .Where(bd => bd.BeginAt >= startDate &&
                                                   bd.EndAt <= endDate &&
                                                   bd.Booking != null &&
                                                   bd.Booking.Status != BookingStatus.Canceled &&
                                                   bd.Booking.Status != BookingStatus.Rejected)
                                        on ts.Id equals bd.TimeSlotId into bookingGroup
                                    from bd in bookingGroup.DefaultIfEmpty()
                                    group new { ts, bd } by new
                                    {
                                        ts.Id,
                                        ts.StartTime,
                                        ts.EndTime,
                                        ts.Price
                                    } into g
                                    select new TimeSlotStatisticDetailDTO
                                    {
                                        TimeSlotId = g.Key.Id,
                                        BeginAt = g.Key.StartTime,
                                        EndAt = g.Key.EndTime,
                                        Price = g.Key.Price,
                                        BookingCount = g.Count(x => x.bd != null),
                                        TotalRevenue = g.Sum(x => x.bd != null ? x.bd.Amonut : 0)
                                    })
                                  .OrderBy(x => x.BeginAt)
                                  .ToListAsync(cancellationToken);

            // Tạo DTO kết quả
            var result = new TimeSlotStatisticDTO
            {
                Year = request.Year,
                Month = request.Month,
                StartDate = startDate,
                EndDate = endDate,
                Details = statistics
            };

            return result;
        }
    }
}
