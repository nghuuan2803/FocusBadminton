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
        public enum ScheduleStatus
        {
            TimeOut = 0,
            Available = 1,
            Holding = 2,
            Pending = 3,
            Booked = 4,
            Completed = 5,
            Paused = 6,
            Blocked = 7
        }
    }
}
