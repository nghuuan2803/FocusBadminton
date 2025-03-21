using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Enums;

namespace Infrastructure.Implements.Payments
{
    public class PaymentAdapterFactory : IPaymentAdapterFactory
    {
        private readonly IServiceProvider _provider;

        public PaymentAdapterFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IPaymentAdapter CreateAdapter(PaymentMethod method) => method switch
        {
            PaymentMethod.Momo => _provider.GetRequiredService<MomoPaymentAdapter>(),
            PaymentMethod.VnPay => _provider.GetRequiredService<VnPayPaymentAdapter>(),
            _ => throw new NotSupportedException($"Payment method {method} not supported")
        };
    }
}
