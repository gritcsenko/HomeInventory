using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.Extensions.DependencyInjection;

public static class DynamicAuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddDynamicAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<PermissionList>();
        services.AddTransient<IAuthorizationHandler, DynamicAuthorizationHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicyNames.Dynamic, static pb => pb
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .AddRequirements(new DynamicPermissionRequirement(static ep => ep.GetPermissions())));

        return services;
    }

    public static TApp UseDynamicAuthorization<TApp>(this TApp app)
        where TApp : IApplicationBuilder, IEndpointRouteBuilder
    {
        var permissionList = app.ApplicationServices.GetRequiredService<PermissionList>();
        foreach (var datasource in app.DataSources)
        {
            foreach (var endpoint in datasource.Endpoints)
            {
                foreach (var permission in endpoint.GetPermissions())
                {
                    permissionList.Add(permission);
                }
            }
        }

        app.UseAuthorization();

        return app;
    }

    public static TBuilder RequireDynamicAuthorization<TBuilder>(this TBuilder builder, params PermissionType[] permissions)
         where TBuilder : IEndpointConventionBuilder =>
        builder.RequireAuthorization(AuthorizationPolicyNames.Dynamic).WithMetadata(new PermissionMetadata(permissions));

    private static IEnumerable<PermissionType> GetPermissions(this Endpoint endpoint) =>
        endpoint.Metadata.GetOrderedMetadata<PermissionMetadata>().SelectMany(static a => a.Permissions);

    private static class AuthorizationPolicyNames
    {
        public const string Dynamic = nameof(Dynamic);
    }
}
