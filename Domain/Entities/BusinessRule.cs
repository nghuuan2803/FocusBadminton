using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class BusinessRule : BaseAuditableEntity<string>
    {
        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Value { get; set; }
        public bool IsApplied { get; set; }
    }
}
