namespace Shared.TimeSlots
{
    public class TimeSlotDTO
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
