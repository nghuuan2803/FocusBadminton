using Sh.Models;
using Domain.Common;

namespace Sh.Interfaces
{
    public interface IMailService
    {
        Task<Result> Send(SendMailRequest request);
    }
}
