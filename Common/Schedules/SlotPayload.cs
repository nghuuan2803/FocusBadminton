using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Schedules
{
    public class SlotPayload
    {
        public int HoldSlotId { get; set; }
        public int CourtId { get; set; }
        public int TimeSlotId { get; set; }
        public int? BookingType { get; set; }
        public DateTimeOffset? BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public string? DayOfWeek { get; set; }
        public string? HeldBy { get; set; }
    }
}
