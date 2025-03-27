namespace Shared.Statistic
{
    public class TimeSlotStatisticDTO
    {
        public int Year { get; set; }
        public int? Month { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public IEnumerable<TimeSlotStatisticDetailDTO> Details { get; set; }
    }
    public class TimeSlotStatisticDetailDTO
    {
        public int TimeSlotId { get; set; }
        public TimeSpan BeginAt { get; set; }
        public TimeSpan EndAt { get; set; }
        public double Price { get; set; }
        public int BookingCount { get; set; }
        public double TotalRevenue { get; set; }
    }
}
