using Application.Interfaces;
using Application.Models.PaymentModels;
using Infrastructure.Services.Momo;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Implements.Payments
{
    public class MomoPaymentAdapter : IPaymentAdapter
    {
        private readonly IMomoService _momoService;
        private readonly IOptions<MomoOptionModel> _options;

        public MomoPaymentAdapter(IMomoService momoService, IOptions<MomoOptionModel> options)
        {
            _momoService = momoService;
            _options = options;
        }

        public async Task<PaymentLinkResponse> GeneratePaymentLinkAsync(PaymentRequest request)
        {
            var orderInfo = new OrderInfoModel
            {
                FullName = request.FullName,
                OrderId = request.OrderId ?? DateTime.UtcNow.Ticks.ToString(),
                OrderInfo = request.OrderInfo,
                Amount = request.Amount
            };

            var momoResponse = await _momoService.CreatePaymentAsync(orderInfo, request.Action);
            if (momoResponse.ErrorCode != 0)
            {
                return new PaymentLinkResponse { IsSuccess = false, ErrorMessage = momoResponse.Message };
            }

            return request.Action == "mobile"
                ? new PaymentLinkResponse { IsSuccess = true, Deeplink = momoResponse.Deeplink }
                : new PaymentLinkResponse { IsSuccess = true, PaymentUrl = momoResponse.PayUrl };
        }

        public async Task<PaymentStatusResponse> VerifyPaymentAsync(PaymentVerificationRequest request)
        {
            if (request.IpnData != null) // Xử lý IPN cho mobile
            {
                var rawData = $"partnerCode={request.IpnData["partnerCode"]}&orderId={request.IpnData["orderId"]}&requestId={request.IpnData["requestId"]}&amount={request.IpnData["amount"]}&transId={request.IpnData["transId"]}&resultCode={request.IpnData["resultCode"]}";
                var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

                if (signature != request.IpnData["signature"])
                {
                    return new PaymentStatusResponse { IsSuccess = false, ErrorMessage = "Invalid signature" };
                }

                return new PaymentStatusResponse
                {
                    IsSuccess = int.Parse(request.IpnData["resultCode"]) == 0,
                    OrderId = request.IpnData["orderId"],
                    Amount = double.Parse(request.IpnData["amount"]),
                    TransactionId = request.IpnData["transId"],
                    ErrorCode = request.IpnData["resultCode"]
                };
            }

            if (request.QueryData != null) // Xử lý redirect cho web
            {
                var result = _momoService.PaymentExecuteAsync(request.QueryData);
                return new PaymentStatusResponse
                {
                    IsSuccess = result.ErrorCode == 0,
                    OrderId = result.OrderId,
                    Amount = double.Parse(result.Amount ?? "0"),
                    ErrorCode = result.ErrorCode.ToString(),
                    OrderInfo = result.OrderInfo
                };
            }

            return new PaymentStatusResponse { IsSuccess = false, ErrorMessage = "No verification data" };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }

}

