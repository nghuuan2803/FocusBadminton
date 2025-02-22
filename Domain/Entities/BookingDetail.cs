namespace Domain.Entities
{
    public class BookingDetail : BaseAuditableEntity<int>
    {
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
        public int CourtId { get; set; }
        public Court? Court { get; set; }
        public int TimeSlotId { get; set; }
        public TimeSlot? TimeSlot { get; set; }
        public DateTimeOffset? BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public string? DayOfWeek { get; set; }
        public double Price { get; set; }
        public double Amonut { get; set; }

    }
}
