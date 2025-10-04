using Microsoft.AspNetCore.Builder;

namespace HomeInventory.Web.Authorization.Dynamic;

public static class DynamicAuthorizationServiceCollectionExtensions
{
    public static TBuilder RequireDynamicAuthorization<TBuilder>(this TBuilder builder, params IReadOnlyCollection<PermissionType> permissions)
        where TBuilder : IEndpointConventionBuilder =>
        builder
            .RequireAuthorization(AuthorizationPolicyNames.Dynamic)
            .WithMetadata(new PermissionMetadata(permissions));
}
