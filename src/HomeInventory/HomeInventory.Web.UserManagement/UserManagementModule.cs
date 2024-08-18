using AutoMapper;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

public class UserManagementModule(IScopeAccessor scopeAccessor) : ApiModule("/api/users/manage")
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .WithValidationOf<RegisterRequest>();
    }

    public async Task<Results<Ok<RegisterResponse>, ProblemHttpResult>> RegisterAsync([FromBody] RegisterRequest body, [FromServices] IUserRepository userRepository, HttpContext context, CancellationToken cancellationToken = default)
    {
        using var _ = _scopeAccessor.Set(userRepository);
        var mapper = _scopeAccessor.GetRequiredContext<IMapper>();
        var hub = _scopeAccessor.GetRequiredContext<IMessageHub>();
        var factory = _scopeAccessor.GetRequiredContext<IProblemDetailsFactory>();

        var command = mapper.MapOrFail<RegisterUserRequestMessage>(body, o => o.State = hub.Context);
        var response = await hub.RequestAsync(command, cancellationToken);

        return await response.Match<Task<Results<Ok<RegisterResponse>, ProblemHttpResult>>>(
            async error =>
            {
                var problem = factory.ConvertToProblem(new Seq<Error>([error]), context.TraceIdentifier);
                var result = TypedResults.Problem(problem);
                return await ValueTask.FromResult(result);
            },
            async () =>
            {
                var query = mapper.MapOrFail<UserIdQueryMessage>(body, o => o.State = hub.Context);
                var queryResult = await hub.RequestAsync(query, cancellationToken);
                return factory.MatchToOk(queryResult, mapper.MapOrFail<RegisterResponse>, context.TraceIdentifier);
            });
    }
}
