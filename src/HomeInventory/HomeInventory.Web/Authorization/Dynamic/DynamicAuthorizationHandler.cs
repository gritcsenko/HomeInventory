using System.Security.Claims;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Authorization.Dynamic;

public class DynamicAuthorizationHandler : AuthorizationHandler<DynamicPermissionRequirement>
{
    private static readonly GuidIdConverter<UserId> _idConverter = new();

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

        if (!Guid.TryParse(idText, out var id))
        {
            context.Fail(new AuthorizationFailureReason(this, $"User has no valid id '{idText}'"));
            return;
        }

        await _idConverter.TryConvert(id)
            .Match(async userId =>
            {
                using var scope = httpContext.RequestServices.CreateScope();
                var provider = scope.ServiceProvider;
                var repository = provider.GetRequiredService<IUserRepository>();

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
                context.Fail(new AuthorizationFailureReason(this, $"User has invalid id: {error.Message}"));
                return Task.CompletedTask;
            });
    }
}
