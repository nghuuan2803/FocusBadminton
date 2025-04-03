using Application.Interfaces;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shared.CostCalculators;
using Shared.Enums;

namespace Application.Features.Bookings.Calculators
{
    public class DiscountCostCalculator : ICostCalculator
    {
        private readonly IRepository<TimeSlot> _timeSlotRepo;
        private readonly IRepository<Court> _courtRepo;
        private ICostCalculator _costCalculator;

        public DiscountCostCalculator(IServiceProvider provider, ICostCalculator costCalculator)
        {
            var scope = provider.CreateScope();
            _timeSlotRepo = scope.ServiceProvider.GetRequiredService<IRepository<TimeSlot>>();
            _courtRepo = scope.ServiceProvider.GetRequiredService<IRepository<Court>>();
            var voucherRepo = scope.ServiceProvider.GetRequiredService<IRepository<Voucher>>();
            _costCalculator = new VoucherCostDecorator(costCalculator,voucherRepo);
        }

        public async Task<double> CalculateAsync(CostCalculatorRequest request)
        {
            return await _costCalculator.CalculateAsync(request);
        }
    }
}
