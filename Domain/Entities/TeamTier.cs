using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class TeamTier : BaseAuditableEntity<int>
    {
        [MaxLength(30)]
        public required string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public double MinPoints { get; set; }
        public double DiscountPercent { get; set; }
        [MaxLength(100)]
        public string? Image { get; set; }
    }
}
