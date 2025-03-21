using Application.Interfaces;
using Application.Models.PaymentModels;
using Infrastructure.Services.VnPay;

namespace Infrastructure.Implements.Payments
{
    public class VnPayPaymentAdapter : IPaymentAdapter
    {
        private readonly IVnPayService _vnPayService;

        public VnPayPaymentAdapter(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        public async Task<PaymentLinkResponse> GeneratePaymentLinkAsync(PaymentRequest request)
        {
            var vnPayRequest = new VnPaymentRequestModel
            {
                FullName = request.FullName,
                Description = request.OrderInfo,
                Amount = request.Amount,
                CreateDate = DateTime.Now,
                BookingId = request.OrderId?.GetHashCode() ?? 0
            };

            var paymentUrl = _vnPayService.CreatePaymentUrl(request.Context, vnPayRequest, request.Action);
            return new PaymentLinkResponse { IsSuccess = true, PaymentUrl = paymentUrl };
        }

        public async Task<PaymentStatusResponse> VerifyPaymentAsync(PaymentVerificationRequest request)
        {
            if (request.QueryData != null) // Chỉ hỗ trợ redirect cho web hiện tại
            {
                var result = _vnPayService.PaymentExecute(request.QueryData);
                return new PaymentStatusResponse
                {
                    IsSuccess = result.Success,
                    OrderId = result.OrderId,
                    Amount = double.Parse(request.QueryData["vnp_Amount"]) / 100,
                    TransactionId = result.TransactionId,
                    ErrorCode = result.VnPayResponseCode
                };
            }

            return new PaymentStatusResponse { IsSuccess = false, ErrorMessage = "No IPN support for VnPay yet" };
        }
    }
}

