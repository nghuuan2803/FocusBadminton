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

        public enum ScheduleStatus
        {
            Availble = 1,
            CanNotHold = 2,
            Holding = 3,
            Booked = 4,
            Paused = 5
        }
    }
}
