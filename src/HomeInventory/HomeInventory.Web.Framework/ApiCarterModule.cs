using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace HomeInventory.Web.Framework;

public abstract class ApiCarterModule : ICarterModule
{
    private readonly Lazy<RoutePattern> _lazyGroupPrefix;

    protected ApiCarterModule() => _lazyGroupPrefix = new(() => RoutePatternFactory.Parse(PathPrefix));

    protected abstract string PathPrefix { get; }

    public RoutePattern GroupPrefix => _lazyGroupPrefix.Value;

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(GroupPrefix);

        AddRoutes(group);
    }

    protected abstract void AddRoutes(RouteGroupBuilder group);
}
