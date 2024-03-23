using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Authorization.Dynamic;

public class DynamicPermissionRequirement(Func<Endpoint, IEnumerable<PermissionType>> extractPermissionsFunc) : IAuthorizationRequirement
{
    private readonly Func<Endpoint, IEnumerable<PermissionType>> _extractPermissionsFunc = extractPermissionsFunc;

    public IEnumerable<PermissionType> GetPermissions(Endpoint endpoint)
    {
        return _extractPermissionsFunc(endpoint);
    }
}
