using Domain.Common;
using Domain.Entities;
using Application.Interfaces;

namespace BadmintonCourtBooking.Infrastructure.Identity
{
    public class AccountManager : IAccountManager
    {
        public Task<Result> ChangePassword(string email, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ConfirmResetPassword(string email, string confirmCode)
        {
            throw new NotImplementedException();
        }

        public Task<Result> LockUser(string email, int? UnlockDate)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Account>> Register(string email, string phone, string fullname, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ResetPassword(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UnlockUser(string email)
        {
            throw new NotImplementedException();
        }
    }
}
