using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Court : BaseAuditableEntity<int>
    {
        [MaxLength(30)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        public int? FacilityId { get; set; }
        public Facility? Facility { get; set; }
        public double Coofficient { get; set; } = 1.0;
        public CourtType Type { get; set; } = CourtType.Normal;

        [MaxLength(500)]
        public string? Images { get; set; }
        public CourtStatus Status { get; set; } = CourtStatus.Available;

    }
}
