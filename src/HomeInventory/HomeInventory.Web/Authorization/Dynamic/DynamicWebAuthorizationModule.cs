using HomeInventory.Modules.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Authorization.Dynamic;

public sealed class DynamicWebAuthorizationModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PermissionList>();
        services.AddTransient<IAuthorizationHandler, DynamicAuthorizationHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicyNames.Dynamic, pb => pb
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .AddRequirements(new DynamicPermissionRequirement(GetPermissions)));
    }

    public override void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
        var permissionList = applicationBuilder.ApplicationServices.GetRequiredService<PermissionList>();
        permissionList.AddRange(endpointRouteBuilder.DataSources.SelectMany(s => s.Endpoints).SelectMany(GetPermissions));

        applicationBuilder.UseAuthorization();
    }

    private static IEnumerable<PermissionType> GetPermissions(Endpoint endpoint) =>
       endpoint.Metadata.GetOrderedMetadata<PermissionMetadata>().SelectMany(a => a.Permissions);
}
