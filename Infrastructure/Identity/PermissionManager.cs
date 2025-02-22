using Sh.Interfaces;
using Domain.Common;

namespace Infrastructure.Identity
{
    public class PermissionManager : IPermissionManager
    {
        public Task<Result> AddRolePermission(string roleName, string permissionName)
        {
            throw new NotImplementedException();
        }

        public Task<Result> AddUserPermission(string email, string permissionName)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveRolePermisson(string roleName, string permissionName)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveUserPermisson(string email, string permissionName)
        {
            throw new NotImplementedException();
        }
    }
}
