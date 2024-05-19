using AutoMapper;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

public class UserManagementModule(IMapper mapper, ISender sender, IProblemDetailsFactory problemDetailsFactory) : ApiModule("/api/users/manage")
{
    private readonly IMapper _mapper = mapper;
    private readonly ISender _sender = sender;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .WithValidationOf<RegisterRequest>();
    }

    public async Task<Results<Ok<RegisterResponse>, ProblemHttpResult>> RegisterAsync([FromBody] RegisterRequest body, HttpContext context, CancellationToken cancellationToken = default)
    {
        var command = _mapper.MapOrFail<RegisterCommand>(body);
        var result = await _sender.Send(command, cancellationToken);
        return await result.Match<Task<Results<Ok<RegisterResponse>, ProblemHttpResult>>>(
            async _ =>
            {
                var query = _mapper.MapOrFail<UserIdQuery>(body);
                var queryResult = await _sender.Send(query, cancellationToken);
                return _problemDetailsFactory.MatchToOk(queryResult, _mapper.MapOrFail<RegisterResponse>, context.TraceIdentifier);
            },
            async error =>
            {
                var problem = _problemDetailsFactory.ConvertToProblem([error], context.TraceIdentifier);
                var result = TypedResults.Problem(problem);
                return await ValueTask.FromResult(result);
            });
    }
}
