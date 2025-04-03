using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Shared.CostCalculators;

namespace Infrastructure.Implements.CostCalculators
{
    public class FixedUnsetEndDateCostCalculator : ICostCalculator
    {
        private readonly IRepository<TimeSlot> _timeSlotRepo;
        private readonly IRepository<Court> _courtRepo;

        public FixedUnsetEndDateCostCalculator(IRepository<TimeSlot> timeSlotRepo, IRepository<Court> courtRepo)
        {
            _timeSlotRepo = timeSlotRepo;
            _courtRepo = courtRepo;
        }

        public async Task<double> CalculateAsync(CostCalculatorRequest request)
        {
            var timeSlot = await _timeSlotRepo.FindAsync(request.TimeSlotId);
            var court = await _courtRepo.FindAsync(request.CourtId);
            if (timeSlot == null || court == null) return -1;

            var firstWeekEnd = request.BeginAt.AddDays(6); // Tuần đầu tiên
            int dayCount = 0;
            for (var date = request.BeginAt; date <= firstWeekEnd; date = date.AddDays(1))
            {
                if (date.ToOffset(TimeSpan.FromHours(7)).DayOfWeek.ToString() == request.DayOfWeek)
                {
                    dayCount++;
                }
            }
            return timeSlot.Price * court.Coofficient * dayCount;
        }
    }
}
