using Asp.Versioning;
using Carter;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

internal abstract class ApiModule : CarterModule
{
    private readonly string _groupPrefix;
    private readonly Permission? _permission;
    private ApiVersion _version = new(1);

    protected ApiModule(string groupPrefix, Permission? permission = null)
    {
        IncludeInOpenApi();
        _groupPrefix = groupPrefix;
        _permission = permission;
    }

    protected void MapToApiVersion(ApiVersion version) => _version = version;

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.GetVersionSet();

        //app.MapGroup("/api/v{version:apiVersion}/...")
        var group = app.MapGroup(_groupPrefix)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(_version);

        if (_permission.HasValue)
        {
            group.RequireDynamicAuthorization(_permission.Value);
        }

        AddRoutes(group);
    }

    protected abstract void AddRoutes(RouteGroupBuilder group);
}
