using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Shared.CostCalculators;

namespace Infrastructure.Implements.CostCalculators
{
    public class WeekendCostDecorator : ICostCalculator
    {
        private readonly ICostCalculator _costCalculator;
        private readonly IRepository<BusinessRule> _ruleRepo;

        public WeekendCostDecorator(ICostCalculator costCalculator, IRepository<BusinessRule> businessRulesRepo)
        {
            _costCalculator = costCalculator;
            _ruleRepo = businessRulesRepo;
        }

        public async Task<double> CalculateAsync(CostCalculatorRequest request)
        {
            double baseCost = await _costCalculator.CalculateAsync(request);
            var dayOfWeek = request.DayOfWeek;
            if (string.IsNullOrEmpty(dayOfWeek)) return baseCost;
            var rules = await _ruleRepo.GetAllAsync(r => r.IsApplied);
            double coefficient = 1.0;

            if (dayOfWeek == "Friday")
                coefficient = double.Parse(rules.FirstOrDefault(r => r.Id == "price_friday")?.Value ?? "1.0");
            else if (dayOfWeek == "Saturday")
                coefficient = double.Parse(rules.FirstOrDefault(r => r.Id == "price_saturday")?.Value ?? "1.0");
            else if (dayOfWeek == "Sunday")
                coefficient = double.Parse(rules.FirstOrDefault(r => r.Id == "price_sunday")?.Value ?? "1.0");

            return baseCost * coefficient;
        }
    }
}
