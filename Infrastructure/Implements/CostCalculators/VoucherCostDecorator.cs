using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Shared.CostCalculators;

namespace Infrastructure.Implements.CostCalculators
{
    public class VoucherCostDecorator : ICostCalculator
    {
        private readonly ICostCalculator _costCalculator;
        private readonly IRepository<Voucher> _voucherRepo;

        public VoucherCostDecorator(ICostCalculator costCalculator, IRepository<Voucher> voucherRepo)
        {
            _costCalculator = costCalculator;
            _voucherRepo = voucherRepo;
        }

        public async Task<double> CalculateAsync(CostCalculatorRequest request)
        {
            double baseCost = await _costCalculator.CalculateAsync(request);
            if (baseCost < 0)
                return -1;
            var voucher = await _voucherRepo.FindAsync(request.VoucherId);
            if (voucher == null)
                return -1;
            voucher.IsUsed = true;
            _voucherRepo.Update(voucher);
            if (voucher.DiscountType == Shared.Enums.DiscountType.Percent)
                //vd: discount = 15%, base cost = 50000 => cost = 50000 - 50000 x 15 / 100 = 42500
                return baseCost - (baseCost * voucher.Value) / 100;
            else
                return baseCost - voucher.Value;
        }
    }
}
