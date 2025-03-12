using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Schedules
{
    public class CheckMultiDayAvailabilityRequest
    {
        public int CourtId { get; set; }
        public List<int> TimeSlotIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> DaysOfWeek { get; set; }
    }
    public class CheckMultiDayAvailabilityResponse
    {
        public Dictionary<int, List<DateTime>> AvailableDates { get; set; } // Key: TimeSlotId, Value: Danh sách ngày khả dụng
        public Dictionary<int, List<DateTime>> LockedDates { get; set; }    // Key: TimeSlotId, Value: Danh sách ngày bị khóa
    }
}
