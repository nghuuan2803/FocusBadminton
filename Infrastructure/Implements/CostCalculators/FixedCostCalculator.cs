using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Shared.CostCalculators;

namespace Infrastructure.Implements.CostCalculators
{
    public class FixedCostCalculator : ICostCalculator
    {
        private readonly IRepository<TimeSlot> _timeSlotRepo;
        private readonly IRepository<Court> _courtRepo;

        public FixedCostCalculator(IRepository<TimeSlot> timeSlotRepo, IRepository<Court> courtRepo)
        {
            _timeSlotRepo = timeSlotRepo;
            _courtRepo = courtRepo;
        }

        public async Task<double> CalculateAsync(CostCalculatorRequest request)
        {
            double result = 0;
            var timeSlot = await _timeSlotRepo.FindAsync(request.TimeSlotId);
            if (timeSlot == null)
                return -1;
            var court = await _courtRepo.FindAsync(request.CourtId);
            if (court == null)
                return -1;
            int dayCount = 0;
            for (var date = request.BeginAt; date <= request.EndAt; date = date.AddDays(1))
            {
                if (date.ToOffset(TimeSpan.FromHours(7)).DayOfWeek.ToString() == request.DayOfWeek)
                {
                    dayCount++;
                }
            }

            result = timeSlot.Price * court.Coofficient * dayCount;

            return result;
        }
    }
}
