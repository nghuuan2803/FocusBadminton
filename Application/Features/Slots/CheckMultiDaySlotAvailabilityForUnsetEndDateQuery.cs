using Application.Interfaces.DapperQueries;
using Domain.Repositories;
using Shared.Enums;

namespace Application.Features.Slots
{
    public class CheckMultiDaySlotAvailabilityForUnsetEndDateQuery : IRequest<List<int>>
    {
        public int CourtId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public List<string> DaysOfWeek { get; set; } = new List<string>();
    }

    public class CheckMultiDaySlotAvailabilityForUnsetEndDateQueryHandler : IRequestHandler<CheckMultiDaySlotAvailabilityForUnsetEndDateQuery, List<int>>
    {
        private readonly IScheduleQueries _schedules;
        private readonly IRepository<TimeSlot> _timeSlotRepo;
        private readonly Logger _logger;

        public CheckMultiDaySlotAvailabilityForUnsetEndDateQueryHandler(
            IScheduleQueries schedules,
            IRepository<TimeSlot> timeSlotRepo)
        {
            _schedules = schedules;
            _timeSlotRepo = timeSlotRepo;
            _logger = Logger.Instance;
        }

        public async Task<List<int>> Handle(CheckMultiDaySlotAvailabilityForUnsetEndDateQuery request, CancellationToken cancellationToken)
        {
            _logger.Log($"Kiểm tra khung giờ khả dụng cho Fixed_Unset_EndDate, CourtId: {request.CourtId}");

            var allTimeSlots = await _timeSlotRepo.GetAllAsync(ts => ts.IsApplied && !ts.IsDeleted);
            var availableSlotIds = new List<int>();

            foreach (var timeSlot in allTimeSlots)
            {
                bool isAvailableForAllDays = true;
                foreach (var dayOfWeek in request.DaysOfWeek)
                {
                    var available = await _schedules.CheckAvailable(
                        request.CourtId,
                        timeSlot.Id,
                        BookingType.Fixed_UnSetEndDate,
                        request.StartDate.ToUniversalTime(),
                        null, // Không cần EndAt
                        dayOfWeek);

                    if (!available)
                    {
                        isAvailableForAllDays = false;
                        break;
                    }
                }

                if (isAvailableForAllDays)
                {
                    availableSlotIds.Add(timeSlot.Id);
                }
            }

            _logger.Log($"Tìm thấy {availableSlotIds.Count} khung giờ khả dụng");
            return availableSlotIds;
        }
    }
}
