using Domain.Common;
using Domain.Entities;

namespace Sh.Interfaces
{
    public interface IAccountManager
    {
        Task<Result<Account>> Register(string email, string phone, string fullname, string password);
        Task<Result> LockUser(string email, int? UnlockDate);
        Task<Result> UnlockUser(string email);
        Task<Result> ResetPassword(string email);
        Task<Result> ConfirmResetPassword(string email, string confirmCode);
        Task<Result> ChangePassword(string email, string oldPassword, string newPassword);
    }
}
