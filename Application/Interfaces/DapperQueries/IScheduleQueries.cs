using Shared.Enums;
using Shared.Schedules;

namespace Application.Interfaces.DapperQueries
{
    public interface IScheduleQueries
    {
        Task<IEnumerable<ScheduleDTO>> GetAllCourtSchedulesAsync(DateTime date, int facilityId);
        Task<IEnumerable<ScheduleDTO>> GetCourtSchedulesAsync(DateTime date, int courtId);
        Task<IEnumerable<CourtScheduleDTO>> GetCourtSchedulesAsync(DateTime beginDate, DateTime endDate, int courtId);

        Task<bool> CheckAvailable(int courtId, int timeSlotId, BookingType bookingType, 
            DateTimeOffset beginAt, DateTimeOffset? endAt, string? dayOfWeek);

    }
}
