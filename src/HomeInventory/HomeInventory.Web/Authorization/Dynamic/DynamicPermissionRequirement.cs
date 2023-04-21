using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Authorization.Dynamic;

public class DynamicPermissionRequirement : IAuthorizationRequirement
{
    private readonly Func<Endpoint, IEnumerable<Permission>> _extractPermissionsFunc;

    public DynamicPermissionRequirement(Func<Endpoint, IEnumerable<Permission>> extractPermissionsFunc)
    {
        _extractPermissionsFunc = extractPermissionsFunc;
    }

    public IEnumerable<Permission> GetPermissions(Endpoint endpoint)
    {
        return _extractPermissionsFunc(endpoint);
    }
}
