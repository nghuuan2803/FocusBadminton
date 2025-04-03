using Application.Features.Bookings.Commands;
using Application.Interfaces;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shared.CostCalculators;

namespace Application.Features.Bookings.Calculators
{
    public class BookingCostProcessor
    {
        private readonly IServiceProvider _provider;
        private readonly IRepository<TimeSlot> _timeSlotRepo;
        private readonly IRepository<Court> _courtRepo;
        private readonly ICostCalculatorFactory _costCalculatorFactory;

        public BookingCostProcessor(
            IServiceProvider serviceProvider,
            ICostCalculatorFactory costCalculatorFactory)
        {
            _provider = serviceProvider;
            _timeSlotRepo = serviceProvider.GetRequiredService<IRepository<TimeSlot>>();
            _courtRepo = serviceProvider.GetRequiredService<IRepository<Court>>();
            _costCalculatorFactory = costCalculatorFactory;
        }

        public async Task<(double totalAmount, double discount)> CalculateBookingCostAsync(
            Booking booking,
            CreateBookingCommand request)
        {
            double totalAmount = 0;
            double finalAmount = 0;

            foreach (var detail in booking.Details)
            {
                var court = await _courtRepo.FindAsync(detail.CourtId);
                var timeSlot = await _timeSlotRepo.FindAsync(detail.TimeSlotId);
                var costRequest = new CostCalculatorRequest
                {
                    CourtId = detail.CourtId,
                    TimeSlotId = detail.TimeSlotId,
                    BeginAt = detail.BeginAt.Value,
                    EndAt = detail.EndAt,
                    DayOfWeek = detail.DayOfWeek,
                    MemberId = request.MemberId,
                    VoucherId = request.VoucherId,
                    BookingType = request.Type
                };

                var totalCostCalculator = _costCalculatorFactory.CreateCalculator(costRequest);
                var finalCostCalculator = new DiscountCostCalculator(_provider,totalCostCalculator);

                double itemBaseCost = await totalCostCalculator.CalculateAsync(costRequest);
                double itemCost = await finalCostCalculator.CalculateAsync(costRequest);

                if (itemCost < 0)
                {
                    throw new Exception($"Không thể tính giá cho CourtId: {detail.CourtId}, TimeSlotId: {detail.TimeSlotId}");
                }
                detail.Price = timeSlot.Price * court.Coofficient; // Giá gốc
                detail.Amonut = itemCost;   // Giá sau giảm giá
                totalAmount += itemBaseCost;
                finalAmount += itemCost;
            }

            double discount = totalAmount - finalAmount;
            return (totalAmount, discount < 0 ? 0 : discount);
        }
    }
}
