using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<string>> ExcuteAsync(Booking booking, Payment payment); 
    }
}
