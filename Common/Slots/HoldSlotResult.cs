using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Slots
{
    public class HoldSlotResult
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public int HoldId { get; set; }
        public double EstimatedCost { get; set; }
    }
}
