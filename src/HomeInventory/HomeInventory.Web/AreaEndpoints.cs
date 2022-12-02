using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web;

internal static class AreaEndpoints
{
    public static IEndpointRouteBuilder UseAreaEndpoints(this IEndpointRouteBuilder builder)
    {
        var areas = builder.MapGroup("/api/areas")
            .WithOpenApi()
            .RequireDynamicAuthorization(Permission.AccessAreas);

        areas.MapGet("", () => { })
            .RequireDynamicAuthorization(Permission.ReadArea);

        return builder;
    }
}
