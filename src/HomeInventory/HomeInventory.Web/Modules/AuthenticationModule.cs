using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

internal class AuthenticationModule : ApiModule
{
    public AuthenticationModule()
        : base("/api/authentication", PermissionType.AccessPermissions)
    {
    }

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapPost("login", LoginAsync)
            .AllowAnonymous()
            .WithValidationOf<LoginRequest>();
    }

    public static async Task<Results<Ok<LoginResponse>, ProblemHttpResult>> LoginAsync(HttpContext context, [FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var mapper = context.GetMapper();

        var query = mapper.Map<AuthenticateQuery>(body);
        var result = await context.GetSender().Send(query, cancellationToken);
        return context.MatchToOk(result, mapper.Map<LoginResponse>);
    }
}
