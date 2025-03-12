using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class DateTimeHelper
    {
        public static DateTimeOffset ToVietNamTime(this DateTimeOffset datetime)
        {
            return datetime.ToOffset(TimeSpan.FromHours(7));
        }
    }
}
