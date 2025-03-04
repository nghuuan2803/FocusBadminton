namespace Shared.Schedules
{
    public class BookingPayload
    {
        public int BookingId { get; set; }
        public int Status { get; set; }
        public int BookingType { get; set; }
        public List<BookingDetailPayload> Details { get; set; } = [];
        public string? BookBy { get; set; }
    }
    public class BookingDetailPayload
    {
        public int CourtId { get; set; }
        public int TimeSlotId { get; set; }
        public DateTimeOffset? BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public string? DayOfWeek { get; set; }
    }
}
