using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Shared.CostCalculators;

namespace Infrastructure.Implements.CostCalculators
{
    public class MemberLevelCostDecorator : ICostCalculator
    {
        private readonly ICostCalculator _costCalculator;
        private readonly IRepository<Member> _memberRepo;

        public MemberLevelCostDecorator(ICostCalculator costCalculator, IRepository<Member> memberRepo)
        {
            _costCalculator = costCalculator;
            _memberRepo = memberRepo;
        }

        public Task<double> CalculateAsync(CostCalculatorRequest request)
        {
            return _costCalculator.CalculateAsync(request);
        }
    }
}
