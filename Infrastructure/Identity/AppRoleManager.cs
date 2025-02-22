using Sh.Interfaces;
using Domain.Common;
using Domain.Entities;

namespace Infrastructure.Identity
{
    public class AppRoleManager : IRoleManager
    {
        public Task<Result<Role>> CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }
        public Task<Result> DeleteRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<Result> AddRole(string email, string role)
        {
            throw new NotImplementedException();
        }


        public Task<Result> RemoveRole(string email, string role)
        {
            throw new NotImplementedException();
        }
    }
}
