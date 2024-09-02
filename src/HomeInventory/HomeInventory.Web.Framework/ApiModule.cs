using Asp.Versioning;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace HomeInventory.Web.Framework;

public abstract class ApiModule : CarterModule
{
    private readonly RoutePattern _groupPrefix;
    private ApiVersion _version = new(1);

    protected ApiModule(string groupPrefix)
    {
        IncludeInOpenApi();
        _groupPrefix = RoutePatternFactory.Parse(groupPrefix);
    }

    public RoutePattern GroupPrefix => _groupPrefix;

    protected void MapToApiVersion(ApiVersion version) => _version = version;

    public sealed override void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.GetVersionSet();

        //app.MapGroup("/api/v{version:apiVersion}/...")
        var group = app.MapGroup(_groupPrefix)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(_version);

        AddRoutes(group);
    }

    protected abstract void AddRoutes(RouteGroupBuilder group);
}
