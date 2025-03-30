namespace Application.Models.PaymentModels
{
    public class PaymentStatusResponse
    {
        public bool IsSuccess { get; set; }
        public string OrderId { get; set; }
        public double Amount { get; set; }
        public string? OrderInfo { get; set; }
        public string TransactionId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int? BookingId { get; set; }
    }
}
