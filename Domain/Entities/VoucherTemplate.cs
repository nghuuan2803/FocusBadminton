using Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
 /// <summary>
 /// Mẫu phiếu giảm giá
 /// </summary>
    public class VoucherTemplate : BaseAuditableEntity<int>, IClone<Voucher>
    {
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; } = DiscountType.Percent;
        public double Value { get; set; }
        public double MaximumValue { get; set; } = 0;
        public int Duration { get; set; } = 30;

        public Voucher Clone()
        {
            return new Voucher
            {
                Name = Name,
                Description = Description,
                DiscountType = DiscountType,
                Value = Value,
                MaximumValue = MaximumValue,
                VoucherTemplateId = Id,
                Expiry = DateTimeOffset.UtcNow.AddDays(Duration), 
                IsUsed = false,
                Code = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()
            };
        }
        public Voucher Clone(string accountId, DateTimeOffset? expiry = null)
        {
            var voucher = Clone(); 
            voucher.AccountId = accountId;
            voucher.Expiry = expiry ?? voucher.Expiry; 
            return voucher;
        }
    }
}
