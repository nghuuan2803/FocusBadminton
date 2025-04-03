using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Shared.CostCalculators;
using Shared.Enums;

namespace Infrastructure.Implements.CostCalculators
{
    public class CostCalculatorFactory : ICostCalculatorFactory
    {
        private readonly IRepository<TimeSlot> _timeSlotRepo;
        private readonly IRepository<Court> _courtRepo;
        private readonly IRepository<Promotion> _promotionRepo;
        private readonly IRepository<BusinessRule> _ruleRepo;
        private readonly IRepository<Member> _memberRepo;

        public CostCalculatorFactory(
        IRepository<TimeSlot> timeSlotRepo,
        IRepository<Court> courtRepo,
        IRepository<Voucher> voucherRepo,
        IRepository<Promotion> promotionRepo,
        IRepository<BusinessRule> ruleRepo,
        IRepository<Member> memberRepo)
        {
            _timeSlotRepo = timeSlotRepo;
            _courtRepo = courtRepo;
            _promotionRepo = promotionRepo;
            _ruleRepo = ruleRepo;
            _memberRepo = memberRepo;
        }

        public ICostCalculator CreateCalculator(CostCalculatorRequest request)
        {
            ICostCalculator calculator;
            switch (request.BookingType)
            {
                case BookingType.InDay:
                    calculator = new InDayCostCalculator(_timeSlotRepo, _courtRepo);
                    break;
                case BookingType.Fixed:
                    calculator = new FixedCostCalculator(_timeSlotRepo, _courtRepo);
                    break;
                case BookingType.Fixed_UnSetEndDate:
                    calculator = new FixedUnsetEndDateCostCalculator(_timeSlotRepo, _courtRepo);
                    break;
                default:
                    calculator = new InDayCostCalculator(_timeSlotRepo, _courtRepo);
                    break;
            }

            // Decorate dựa trên request
            if (!string.IsNullOrEmpty(request.DayOfWeek))
                calculator = new WeekendCostDecorator(calculator, _ruleRepo);
            if (request.MemberId.HasValue)
                calculator = new MemberLevelCostDecorator(calculator, _memberRepo);
            if (request.PromotionId.HasValue)
                calculator = new PromotionCostDecorator(calculator, _promotionRepo);

            return calculator;
        }
    }
}
