using Application.Interfaces;
using Domain.Repositories;
using Shared.CostCalculators;

namespace Application.Features.Bookings.Calculators
{
    public class PromotionCostDecorator : ICostCalculator
    {
        private readonly ICostCalculator _costCalculator;
        private readonly IRepository<Promotion> _promotionRepo;

        public PromotionCostDecorator(ICostCalculator costCalculator, IRepository<Promotion> promotionRepo)
        {
            _costCalculator = costCalculator;
            _promotionRepo = promotionRepo;
        }

        public async Task<double> CalculateAsync(CostCalculatorRequest request)
        {
            double baseCost = await _costCalculator.CalculateAsync(request);
            if (baseCost < 0)
                return -1;
            var promotion = await _promotionRepo.FindAsync(request.PromotionId);
            if (promotion == null)
                return -1;

            if (promotion.DiscountType == Shared.Enums.DiscountType.Percent)
            {
                return baseCost - baseCost * promotion.DiscountValue / 100;
            }
            return baseCost - promotion.DiscountValue;
        }
    }
}
