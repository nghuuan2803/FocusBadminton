using Application.Common;
using Application.Features.Bookings.Commands;
using Application.Interfaces;
using Shared.CostCalculators;

namespace Application.Features.Bookings
{
    public class BookingCostProcessor
    {
        private readonly IServiceProvider _provider;
        private readonly ICostCalculatorFactory _costCalculatorFactory;

        public BookingCostProcessor(
            IServiceProvider provider,
            ICostCalculatorFactory costCalculatorFactory)
        {
            _provider = provider;
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

                var totalCostCalculator = new BaseCostCalculator(_provider);
                var finalCostCalculator = _costCalculatorFactory.CreateCalculator(costRequest);

                double itemBaseCost = await totalCostCalculator.CalculateAsync(costRequest);
                double itemCost = await finalCostCalculator.CalculateAsync(costRequest);

                if (itemCost < 0)
                {
                    throw new Exception($"Không thể tính giá cho CourtId: {detail.CourtId}, TimeSlotId: {detail.TimeSlotId}");
                }

                totalAmount += itemBaseCost;
                finalAmount += itemCost;
            }

            double discount = totalAmount - finalAmount;
            return (totalAmount, discount < 0 ? 0 : discount);
        }
    }
}
