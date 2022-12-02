using System.Security.Claims;
using FluentResults;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace HomeInventory.Web.Authorization.Dynamic;

public class DynamicAuthorizationHandler : AuthorizationHandler<DynamicPermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicPermissionRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail(new AuthorizationFailureReason(this, "Context has invalid resource"));
            return;
        }

        if (httpContext.GetEndpoint() is not Endpoint endpoint)
        {
            context.Fail(new AuthorizationFailureReason(this, "HTTP context has ho endpoint"));
            return;
        }

        if (context.User.FindFirstValue(ClaimTypes.NameIdentifier) is not string idText)
        {
            context.Fail(new AuthorizationFailureReason(this, "User has no id"));
            return;
        }

        using var scope = httpContext.RequestServices.CreateScope();

        await CreateId<UserId>(scope, idText).Match(
            async userId =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var permissions = requirement.GetPermissions(endpoint);
                foreach (var permission in permissions)
                {
                    if (await repository.HasPermissionAsync(userId, permission.ToString(), httpContext.RequestAborted))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }

                context.Fail(new AuthorizationFailureReason(this, $"User has no permission"));
            },
            error =>
            {
                context.Fail(new AuthorizationFailureReason(this, $"User has invalid id: {string.Join(';', error.Reasons.Select(r => r.Message))}"));
                return Task.CompletedTask;
            });
    }

    private static OneOf<TId, IError> CreateId<TId>(IServiceScope scope, string idText)
        where TId : IIdentifierObject<TId> =>
        scope.ServiceProvider.GetRequiredService<IIdFactory<TId, string>>().CreateFrom(idText);
}
