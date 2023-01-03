using Carter;
using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web;

internal class AreaModule : CarterModule
{
    public AreaModule()
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var areas = app.MapGroup("/api/areas")
            .WithOpenApi()
            .RequireDynamicAuthorization(Permission.AccessAreas);

        areas.MapGet("", () => { })
            .RequireDynamicAuthorization(Permission.ReadArea);
    }
}
