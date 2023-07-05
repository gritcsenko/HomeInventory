using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

public class PermissionModule : ApiModule
{
    public PermissionModule()
        : base("/api/permissions")
    {
    }

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapGet("", GetPermissionsAsync)
            .RequireDynamicAuthorization(PermissionType.ReadPermission);
    }

    public static Task<Ok<IEnumerable<string>>> GetPermissionsAsync([FromServices] PermissionList list, CancellationToken cancellationToken = default)
        => Task.FromResult(TypedResults.Ok(list.Select(p => p.ToString())));
}
