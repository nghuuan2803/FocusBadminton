using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Promotion : BaseAuditableEntity<int>
    {
        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; } = DiscountType.Percent;
        public double DiscountValue { get; set; }

        [MaxLength(100)]
        public string? Facilities { get; set; }

        [MaxLength(50)]
        public string? TiersRequired { get; set; }
        public double? MaximunValue { get; set; }
        public DateTimeOffset BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }

        [MaxLength(500)]
        public string? Images { get; set; }
    }
}
