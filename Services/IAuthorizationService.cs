using System.Security.Claims;

namespace CatalogueService.Services
{
    public interface IPermissionService
    {
        bool HasPermission(ClaimsPrincipal user, string permission);
        bool HasAnyPermission(ClaimsPrincipal user, params string[] permissions);
        bool HasAllPermissions(ClaimsPrincipal user, params string[] permissions);
    }
}
