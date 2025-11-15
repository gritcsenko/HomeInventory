using System.Security.Claims;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;
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
            Fail("Context has invalid resource");
            return;
        }

        if (httpContext.GetEndpoint() is not { } endpoint)
        {
            Fail("HTTP context has ho endpoint");
            return;
        }

        if (context.User.FindFirstValue(ClaimTypes.NameIdentifier) is not { } idText)
        {
            Fail($"User has no {ClaimTypes.NameIdentifier} claim");
            return;
        }

        if (!Ulid.TryParse(idText, out var id))
        {
            Fail($"User has {ClaimTypes.NameIdentifier} = '{idText}' with unknown format");
            return;
        }

        _ = await UserId.Converter.TryConvert(id)
            .MatchAsync(async userId =>
            {
                using var scope = httpContext.RequestServices.CreateScope();
                var provider = scope.ServiceProvider;
                var repository = provider.GetRequiredService<IUserRepository>();

                var permissions = requirement.GetPermissions(endpoint).ToAsyncEnumerable();
                var hasPermission = await permissions.AnyAsync(async (p, ct) => await repository.HasPermissionAsync(userId, p.ToString(), ct), httpContext.RequestAborted);
                if (hasPermission)
                {
                    context.Succeed(requirement);
                    return Unit.Default;
                }

                Fail("User has no permission");
                return Unit.Default;
            },
            errors =>
            {
                Fail(errors.Head.Message);
                return Unit.Default;
            });

        void Fail(string reason) => context.Fail(new(this, reason));
    }
}
