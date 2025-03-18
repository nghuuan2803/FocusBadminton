using Application.Interfaces;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shared.CostCalculators;
using Shared.Enums;

namespace Application.Common
{
    public class BaseCostCalculator : ICostCalculator
    {
        private readonly IRepository<TimeSlot> _timeSlotRepo;
        private readonly IRepository<Court> _courtRepo;

        public BaseCostCalculator(IServiceProvider provider)
        {
            _timeSlotRepo = provider.GetRequiredService<IRepository<TimeSlot>>();
            _courtRepo = provider.GetRequiredService<IRepository<Court>>();
        }

        public async Task<double> CalculateAsync(CostCalculatorRequest request)
        {
            var timeSlot = await _timeSlotRepo.FindAsync(request.TimeSlotId);
            var court = await _courtRepo.FindAsync(request.CourtId);
            if (timeSlot == null || court == null)
                return -1;
            switch (request.BookingType)
            {
                case BookingType.InDay:
                    return timeSlot.Price * court.Coofficient;
                case BookingType.Fixed:
                    int dayCount = 0;
                    for (var date = request.BeginAt; date <= request.EndAt; date = date.AddDays(1))
                    {
                        if (date.ToOffset(TimeSpan.FromHours(7)).DayOfWeek.ToString() == request.DayOfWeek)
                        {
                            dayCount++;
                        }
                    }
                    return timeSlot.Price * court.Coofficient * dayCount;
                case BookingType.Fixed_UnSetEndDate:
                    return timeSlot.Price * court.Coofficient * 7;
                default:
                    return -1;
            }
        }
    }
}
