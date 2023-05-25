using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

internal class UserManagementModule : ApiModule
{
    public UserManagementModule()
        : base("/api/users/manage")
    {
    }

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .WithValidationOf<RegisterRequest>();
    }

    public static async Task<IResult> RegisterAsync(HttpContext context, [FromBody] RegisterRequest body, CancellationToken cancellationToken = default)
    {
        var mapper = context.GetMapper();

        var command = mapper.Map<RegisterCommand>(body);
        var result = await context.GetSender().Send(command, cancellationToken);
        return await result.Match(
            async _ =>
            {
                var query = mapper.Map<UserIdQuery>(body);
                var queryResult = await context.GetSender().Send(query, cancellationToken);
                return context.MatchToOk(queryResult, mapper.Map<RegisterResponse>);
            },
            error => Task.FromResult(context.Problem(error)));
    }
}
