using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Authorization.Dynamic;

public static class DependencyInjection
{
    public static IServiceCollection AddDynamicAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<PermissionList>();
        services.AddTransient<IAuthorizationHandler, DynamicAuthorizationHandler>();

        // Read https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-6.0
        services.AddAuthorization(b =>
        {
            b.AddPolicy(AuthorizationPolicyNames.Dynamic, pb => pb
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .AddRequirements(new DynamicPermissionRequirement(ep => ep.GetPermissions())));
        });

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
        endpoint.Metadata.GetOrderedMetadata<PermissionMetadata>().SelectMany(a => a.Permissions);

    private static class AuthorizationPolicyNames
    {
        public static readonly string Dynamic = nameof(Dynamic);
    }
}
