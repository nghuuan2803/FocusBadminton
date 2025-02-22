namespace Domain.Entities
{
    /// <summary>
    /// Khung giờ
    ///</summary>
    public class TimeSlot : BaseAuditableEntity<int>
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public double Price { get; set; } = 50000;
        public double Duration { get; set; } = 1.0;
        public bool IsApplied { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
