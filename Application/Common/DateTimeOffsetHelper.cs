using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public static class DateTimeOffsetHelper
    {
        public static DateTimeOffset ToDateTimeOffset(this DateTimeOffset? dateTimeOffset)
        {
            return dateTimeOffset.HasValue ? dateTimeOffset.Value : default(DateTimeOffset);

        }
    }
}
