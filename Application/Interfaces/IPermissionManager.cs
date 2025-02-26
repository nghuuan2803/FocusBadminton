using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPermissionManager
    {
        Task<Result> AddUserPermission(string email, string permissionName);
        Task<Result> AddRolePermission(string roleName, string permissionName);

        Task<Result> RemoveUserPermisson(string email,string permissionName);
        Task<Result> RemoveRolePermisson(string roleName,string permissionName);
    }
}
