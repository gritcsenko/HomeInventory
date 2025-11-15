using Asp.Versioning;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace HomeInventory.Web.Framework;

public abstract class ApiCarterModule : ICarterModule
{
    private ApiVersion _version = new(1);
    private readonly Lazy<RoutePattern> _lazyGroupPrefix;

    protected ApiCarterModule() => _lazyGroupPrefix = new(() => RoutePatternFactory.Parse(PathPrefix));

    protected abstract string PathPrefix { get; }

    public RoutePattern GroupPrefix => _lazyGroupPrefix.Value;

    protected void MapToApiVersion(ApiVersion version) => _version = version;

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.GetVersionSet();

        //app.MapGroup("/api/v{version:apiVersion}/...")
        var group = app.MapGroup(GroupPrefix)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(_version);

        AddRoutes(group);
    }

    protected abstract void AddRoutes(RouteGroupBuilder group);
}
