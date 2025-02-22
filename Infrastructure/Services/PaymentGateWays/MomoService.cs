using Sh.Interfaces;
using Domain.Common;
using Domain.Entities;

namespace Infrastructure.Services.PaymentGateWays
{
    public class MomoService : IPaymentService
    {
        public Task<Result<string>> ExcuteAsync(Booking booking, Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}
