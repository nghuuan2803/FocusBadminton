using Application.Models;

namespace Application.Interfaces
{
    public interface IMailService
    {
        Task<Result> Send(SendMailRequest request);
    }
}
