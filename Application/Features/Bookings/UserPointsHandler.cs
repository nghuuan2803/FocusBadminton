using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shared.Enums;

namespace Application.Features.Bookings
{
    public class UserPointsHandler
    {
        private readonly IRepository<Account> _accRepo;
        private readonly IRepository<VoucherTemplate> _templateRepo;
        private readonly IRepository<Voucher> _voucherRepo;
        private readonly IRepository<Member> _memberRepo;

        public UserPointsHandler(IServiceProvider serviceProvider)
        {
            _accRepo = serviceProvider.GetRequiredService<IRepository<Account>>();
            _templateRepo = serviceProvider.GetRequiredService<IRepository<VoucherTemplate>>();
            _voucherRepo = serviceProvider.GetRequiredService<IRepository<Voucher>>();
            _memberRepo = serviceProvider.GetRequiredService<IRepository<Member>>();
        }

        public async Task Handle(double amount, int memberId)
        {
            var member = await _memberRepo.FindAsync(memberId);
            if (member == null)
                return;

            var userAccount = await _accRepo.FindAsync(member.AccountId);
            if (userAccount == null)
                return;

            // Tính điểm nhận được từ số tiền chi tiêu
            double point = amount * 0.01;
            double previousPoints = userAccount.PersonalPoints;
            userAccount.PersonalPoints += point;
            _accRepo.Update(userAccount);

            // Xác định mốc trước và sau khi cộng điểm
            int previousMilestone = (int)(previousPoints / 500) * 500;
            int newMilestone = (int)(userAccount.PersonalPoints / 500) * 500;

            // Nếu user vượt qua một mốc mới
            if (newMilestone > previousMilestone)
            {
                int rewardMilestone = 0;
                int discountValue = 0;

                if (newMilestone % 5000 == 0)
                {
                    rewardMilestone = 5000;
                    discountValue = 20; // Mốc 5000, 10000,... nhận voucher 30%
                }
                else if (newMilestone % 1000 == 0)
                {
                    rewardMilestone = 1000;
                    discountValue = 15; // Mốc 1000, 2000, 3000,... nhận voucher 20%
                }
                else if (newMilestone % 500 == 0)
                {
                    rewardMilestone = 500;
                    discountValue = 10; // Mốc 500, 1500, 2500,... nhận voucher mặc định
                }

                if (rewardMilestone > 0)
                {
                    var voucherTemplate = await _templateRepo.FindAsync(v => v.DiscountType == DiscountType.Percent && v.Value == discountValue);
                    if (voucherTemplate != null)
                    {
                        var voucher = voucherTemplate.Clone();
                        voucher.AccountId = member.AccountId;
                        await _voucherRepo.AddAsync(voucher);
                        await _voucherRepo.SaveAsync();
                    }
                }
            }
        }
    }
}
