namespace Shared.Schedules
{
    public class ScheduleDTO
    {
        public int CourtId { get; set; }
        public string CourtName { get; set; }
        public int TimeSlotId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public ScheduleStatus Status { get; set; }
        public int? HoldId { get; set; }
        public string? HeldBy { get; set; }
        public int? BookingId { get; set; }
        public int? BookingDetailId { get; set; }
    }
}
