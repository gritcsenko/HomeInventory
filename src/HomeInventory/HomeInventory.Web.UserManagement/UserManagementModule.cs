using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Web.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

public class UserManagementModule : ApiModule
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

    public static async Task<Results<Ok<RegisterResponse>, ProblemHttpResult>> RegisterAsync([FromBody] RegisterRequest body, HttpContext context, CancellationToken cancellationToken = default)
    {
        var mapper = context.GetMapper();

        var command = mapper.MapOrFail<RegisterCommand>(body);
        var result = await context.GetSender().Send(command, cancellationToken);
        return await result.Match<Task<Results<Ok<RegisterResponse>, ProblemHttpResult>>>(
            async _ =>
            {
                var query = mapper.MapOrFail<UserIdQuery>(body);
                var queryResult = await context.GetSender().Send(query, cancellationToken);
                return context.MatchToOk(queryResult, mapper.MapOrFail<RegisterResponse>);
            },
            async error => await ValueTask.FromResult(context.Problem(error)));
    }
}
