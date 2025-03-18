using Shared.Enums;

namespace Shared.CostCalculators
{
    public class CostCalculatorRequest
    {
        public int CourtId { get; set; }
        public int TimeSlotId { get; set; }
        public DateTimeOffset BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public string DayOfWeek { get; set; }
        public int? MemberId { get; set; }
        public int? TeamId { get; set; }
        public int? VoucherId { get; set; }
        public int? PromotionId { get; set; }
        public BookingType BookingType { get; set; } = BookingType.InDay;
    }
}
