using Shared.Enums;

namespace Application.Interfaces
{
    public interface IPaymentAdapterFactory
    {
        IPaymentAdapter CreateAdapter(PaymentMethod method);
    }
}
