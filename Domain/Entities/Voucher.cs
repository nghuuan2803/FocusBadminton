using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Voucher : BaseAuditableEntity<int>
    {
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; } = DiscountType.Percent;
        public double Value { get; set; }
        public double MaximumValue { get; set; } = 0;
        public int? VoucherTemplateId { get; set; }
        public VoucherTemplate? VoucherTemplate { get; set; }
        public string? AccountId { get; set; }
        public Account? Account { get; set; }
        public DateTimeOffset? Expiry { get; set; }
        public bool IsUsed { get; set; }

        [MaxLength(36)]
        public string? Origin { get; set; }

        [MaxLength(36)]
        public string? Code { get; set; }
    }
}
