using Asp.Versioning.Conventions;
using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web;

internal class PermissionModule : ApiModule
{
    public PermissionModule()
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = GetVersionSet(app);

        var group = app.MapGroup("/api/permissions")
            .WithOpenApi()
            .RequireDynamicAuthorization(Permission.AccessPermissions)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1);


        group.MapGet("", GetPermissionsAsync)
            .RequireDynamicAuthorization(Permission.ReadPermission);
    }

    public static Task<IResult> GetPermissionsAsync([FromServices] PermissionList list, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IResult>(TypedResults.Ok(list.Select(p => p.ToString())));
    }
}
