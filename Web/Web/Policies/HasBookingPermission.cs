using Microsoft.AspNetCore.Authorization;

namespace Web.Policies
{
    public class HasBookingPermission : IAuthorizationRequirement
    {
        public string Permission { get;}
        public HasBookingPermission(string permission)
        {
            Permission = permission;
        }
    }

    public class HasBookingPermissionHandler : AuthorizationHandler<HasBookingPermission>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasBookingPermission requirement)
        {
            var user = context.User;

            var cofirmBookingPermission = user.FindFirst(c => c.Type == requirement.Permission);

            if (cofirmBookingPermission != null || user.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
