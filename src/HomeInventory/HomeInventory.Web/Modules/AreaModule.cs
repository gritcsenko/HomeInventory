using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

internal class AreaModule : ApiModule
{
    public AreaModule()
        : base("/api/areas", Permission.AccessAreas)
    {
    }

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapGet("", (HttpContext context) =>
        {
            var apiVersion = context.GetRequestedApiVersion();
        })
            .RequireDynamicAuthorization(Permission.ReadArea);
    }
}
