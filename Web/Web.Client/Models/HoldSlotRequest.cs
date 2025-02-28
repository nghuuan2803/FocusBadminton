using Shared.Enums;

namespace Web.Client.Models
{
    public class HoldSlotRequest
    {
        public int CourtId { get; set; }
        public int TimeSlotId { get; set; }
        public string? HoldBy { get; set; }
        public BookingType BookingType { get; set; }
        public DateTimeOffset BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public string? DayOfWeek { get; set; }
    }
}
