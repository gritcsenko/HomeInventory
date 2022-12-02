using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Authorization.Dynamic;

internal static class PermissionEndpoints
{
    public static IEndpointRouteBuilder UsePermissionsEndpoints(this IEndpointRouteBuilder builder)
    {
        var areas = builder.MapGroup("/api/permissions")
            .WithOpenApi()
            .RequireDynamicAuthorization(Permission.AccessPermissions);

        areas.MapGet("", ([FromServices] PermissionList list) => TypedResults.Ok(list.Select(p => p.ToString())))
            .RequireDynamicAuthorization(Permission.ReadPermission);

        return builder;
    }
}
