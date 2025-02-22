using Sh.Interfaces;
using Sh.Models;
using Domain.Common;

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
