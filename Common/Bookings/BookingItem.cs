using System.ComponentModel.DataAnnotations;

namespace Shared.Bookings
{
    public class BookingItem
    {
        public int Id { get; set; }
        public int CourtId { get; set; }
        public string? CourtName { get; set; }
        public int TimeSlotId { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTimeOffset? BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        [MaxLength(10)]
        public string? DayOfWeek { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }
}
