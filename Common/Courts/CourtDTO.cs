using Shared.Enums;

namespace Shared.Courts
{
    public class CourtDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? FacilityId { get; set; }
        public string? FacilityName { get; set; }
        public double Coofficient { get; set; } = 1.0;
        public CourtType Type { get; set; }
        public string? TypeName { get; set; }
        public string? Images { get; set; }
        public CourtStatus Status { get; set; }
    }
}
