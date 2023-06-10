using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Authorization.Dynamic;

public class DynamicPermissionRequirement : IAuthorizationRequirement
{
    private readonly Func<Endpoint, IEnumerable<PermissionType>> _extractPermissionsFunc;

    public DynamicPermissionRequirement(Func<Endpoint, IEnumerable<PermissionType>> extractPermissionsFunc)
    {
        _extractPermissionsFunc = extractPermissionsFunc;
    }

    public IEnumerable<PermissionType> GetPermissions(Endpoint endpoint)
    {
        return _extractPermissionsFunc(endpoint);
    }
}
