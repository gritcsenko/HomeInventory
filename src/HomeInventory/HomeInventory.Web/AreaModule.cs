using Asp.Versioning.Conventions;
using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web;

internal class AreaModule : ApiModule
{
    public AreaModule()
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = GetVersionSet(app);

        //var areas = app.MapGroup("/api/v{version:apiVersion}/areas")
        var group = app.MapGroup("/api/areas")
            .WithOpenApi()
            .RequireDynamicAuthorization(Permission.AccessAreas)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);

        group.MapGet("", (HttpContext context) =>
        {
            var apiVersion = context.GetRequestedApiVersion();
        })
            .RequireDynamicAuthorization(Permission.ReadArea);
    }
}
