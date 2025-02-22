using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// Mẫu phiếu giảm giá
    /// </summary>
    public class VoucherTemplate : BaseAuditableEntity<int>
    {
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; } = DiscountType.Percent;
        public double Value { get; set; }
        public double MaximumValue { get; set; } = 0;
        public int Duration { get; set; } = 30;
    }
}
