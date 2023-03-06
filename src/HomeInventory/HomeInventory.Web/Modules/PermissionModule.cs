using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

internal class PermissionModule : ApiModule
{
    public PermissionModule()
        : base("/api/permissions", Permission.AccessPermissions)
    {
    }

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapGet("", GetPermissionsAsync)
            .RequireDynamicAuthorization(Permission.ReadPermission);
    }

    public static Task<IResult> GetPermissionsAsync([FromServices] PermissionList list, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IResult>(TypedResults.Ok(list.Select(p => p.ToString())));
    }
}
