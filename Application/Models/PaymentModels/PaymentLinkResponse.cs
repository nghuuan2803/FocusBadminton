namespace Application.Models.PaymentModels
{
    public class PaymentLinkResponse
    {
        public bool IsSuccess { get; set; }
        public string PaymentUrl { get; set; } // Dùng cho web
        public string Deeplink { get; set; } // Dùng cho mobile
        public string ErrorMessage { get; set; }
    }
}
