using Application.Models.PaymentModels;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPaymentAdapter
    {
        Task<PaymentLinkResponse> GeneratePaymentLinkAsync(PaymentRequest request);
        Task<PaymentStatusResponse> VerifyPaymentAsync(PaymentVerificationRequest request);
    }
}
