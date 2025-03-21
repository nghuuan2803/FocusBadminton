namespace Application.Models.PaymentModels
{
    public class PaymentResult
    {
        public string OrderId { get; set; }
        public double Amount { get; set; }
        public string OrderInfo { get; set; }
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string ErrorCode { get; set; }
    }
}
