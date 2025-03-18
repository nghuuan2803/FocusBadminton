using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Shared.CostCalculators;

namespace Infrastructure.Implements.CostCalculators
{
    public class InDayCostCalculator : ICostCalculator
    {
        private readonly IRepository<TimeSlot> _timeSlotRepo;
        private readonly IRepository<Court> _courtRepo;

        public InDayCostCalculator(IRepository<TimeSlot> timeSlotRepo, IRepository<Court> courtRepo)
        {
            _timeSlotRepo = timeSlotRepo;
            _courtRepo = courtRepo;
        }

        public virtual async Task<double> CalculateAsync(CostCalculatorRequest request)
        {
            var timeSlot = await _timeSlotRepo.FindAsync(request.TimeSlotId);
            if (timeSlot == null)
                return -1;
            var court = await _courtRepo.FindAsync(request.CourtId);
            if (court == null)
                return -1;
            //giá khung giờ x hệ số giá của sân)
            double result = timeSlot.Price * court.Coofficient;

            return result;
        }
    }
}
