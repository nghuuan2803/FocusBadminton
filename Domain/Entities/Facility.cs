using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Facility : BaseAuditableEntity<int>
    {
        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(20)]
        public string? Latitude { get; set; }
        [MaxLength(20)]
        public string? Longitude { get; set; }

        [MaxLength(50)]
        public string? PlaceId { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? Layout { get; set; }
        public double Coofficient { get; set; } = 1.0;

        [MaxLength(500)]
        public string? Images { get; set; }

        [MaxLength(36)]
        public string? ManagerId { get; set; }

        public FacilityStatus Status { get; set; } = FacilityStatus.Available;
        public ICollection<Court>? Courts { get; set; }
        public enum FacilityStatus
        {
            UnderMaintenance = 0,
            Available = 1,
        }
    }
}
