using Microsoft.AspNetCore.Http;

namespace Application.Models.PaymentModels
{
    public class PaymentVerificationRequest
    {
        public IDictionary<string, string> IpnData { get; set; } // Dữ liệu từ IPN
        public IQueryCollection QueryData { get; set; } // Dữ liệu từ callback
    }
}
