using System.Security.Claims;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Authorization.Dynamic;

internal class DynamicAuthorizationHandler : AuthorizationHandler<DynamicPermissionRequirement>
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

        if (!Ulid.TryParse(idText, out var id))
        {
            context.Fail(new AuthorizationFailureReason(this, $"User has no valid id '{idText}'"));
            return;
        }

        await UserId.Converter.TryConvert(id)
            .MatchAsync(async userId =>
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
                        return Unit.Default;
                    }
                }

                context.Fail(new AuthorizationFailureReason(this, $"User has no permission"));
                return Unit.Default;
            },
            errors =>
            {
                context.Fail(new AuthorizationFailureReason(this, errors.Head.Message));
                return Unit.Default;
            });
    }
}
