using System.Security.Claims;

namespace CatalogueService.Services
{
    public class Auth0PermissionService : IPermissionService
    {
        public bool HasPermission(ClaimsPrincipal user, string permission)
        {
            return user.HasClaim(c => c.Type == "permissions" && c.Value == permission);
        }

        public bool HasAnyPermission(ClaimsPrincipal user, params string[] permissions)
        {
            return permissions.Any(permission => HasPermission(user, permission));
        }

        public bool HasAllPermissions(ClaimsPrincipal user, params string[] permissions)
        {
            return permissions.All(permission => HasPermission(user, permission));
        }
    }
}
