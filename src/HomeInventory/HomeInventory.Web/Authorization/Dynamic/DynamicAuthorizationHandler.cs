using System.Security.Claims;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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

        if (!Guid.TryParse(idText, out var id))
        {
            context.Fail(new AuthorizationFailureReason(this, $"User has no valid id '{idText}'"));
            return;
        }

        using var scope = httpContext.RequestServices.CreateScope();
        var sp = scope.ServiceProvider;
        var converter = new GuidIdConverter<UserId>();
        await converter.Convert(id)
            .Match(async userId =>
            {
                var repository = sp.GetRequiredService<IUserRepository>();

                var permissions = requirement.GetPermissions(endpoint);
                foreach (var permission in permissions)
                {
                    if (await repository.HasAsync(UserSpecifications.HasId(userId), httpContext.RequestAborted))
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
