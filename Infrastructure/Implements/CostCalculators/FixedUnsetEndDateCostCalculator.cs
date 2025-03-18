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
            if (timeSlot == null)
                return -1;
            var court = await _courtRepo.FindAsync(request.CourtId);
            if (court == null)
                return -1;
            return timeSlot.Price * court.Coofficient * 7;
        }
    }
}
