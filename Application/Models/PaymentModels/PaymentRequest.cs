using Microsoft.AspNetCore.Http;

namespace Application.Models.PaymentModels
{
    public class PaymentRequest
    {
        public string FullName { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public double Amount { get; set; }
        public string Action { get; set; } // "mobile" hoặc "web"
        public HttpContext Context { get; set; } // Dùng cho VnPay hoặc các cổng cần IP
    }
}
