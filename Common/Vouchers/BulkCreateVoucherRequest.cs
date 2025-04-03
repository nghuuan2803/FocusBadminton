using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Vouchers
{
    public class BulkCreateVoucherRequest
    {
        public int VoucherTemplateId { get; set; }
        public List<string> AccountIds { get; set; } = new();
        public DateTimeOffset? Expiry { get; set; }
    }
}
