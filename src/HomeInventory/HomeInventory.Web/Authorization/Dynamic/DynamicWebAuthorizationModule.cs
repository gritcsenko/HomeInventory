using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Authorization.Dynamic;

public sealed class DynamicWebAuthorizationModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services
            .AddSingleton<PermissionList>()
            .AddTransient<IAuthorizationHandler, DynamicAuthorizationHandler>()
            .AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicyNames.Dynamic, static pb => pb
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .AddRequirements(new DynamicPermissionRequirement(GetPermissions)));
    }

    public override async Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken = default)
    {
        await base.BuildAppAsync(context, cancellationToken);

        var permissionList = context.GetRequiredService<PermissionList>();
        permissionList.AddRange(context.EndpointRouteBuilder.DataSources.SelectMany(static s => s.Endpoints).SelectMany(GetPermissions));

        context.ApplicationBuilder.UseAuthorization();
    }

    private static IEnumerable<PermissionType> GetPermissions(Endpoint endpoint) =>
        endpoint.Metadata.GetOrderedMetadata<PermissionMetadata>().SelectMany(static a => a.Permissions);
}
