using Domain.Common;
using Application.Interfaces;
using Application.Models;

namespace Infrastructure.Services
{
    public class MailService : IMailService
    {
        public Task<Result> Send(SendMailRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
