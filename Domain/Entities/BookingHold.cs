namespace Domain.Entities
{
    public class BookingHold : BaseEntity<int>
    {
        public int CourtId { get; set; }
        public Court? Court { get; set; }
        public int TimeSlotId { get; set; }
        public TimeSlot? TimeSlot { get; set; }
        public string HeldBy { get; set; }
        public DateTimeOffset HeldAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public BookingType BookingType { get; set; } = BookingType.InDay;
        public DateTimeOffset? BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public string DayOfWeek { get; set; }
    }
}
