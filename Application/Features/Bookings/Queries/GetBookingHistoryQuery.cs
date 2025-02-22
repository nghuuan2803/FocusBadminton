using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Bookings.Queries
{
    class GetBookingHistoryQuery
    {
        public int MemberId { get; set; }
        public int? TeamId { get; set; }
    }
}
