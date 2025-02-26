using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRoleManager
    {
        Task<Result<Role>> CreateRole(string roleName);
        Task<Result> DeleteRole(string roleName);

        Task<Result> AddRole(string email, string role);
        Task<Result> RemoveRole(string email, string role);
    }
}
