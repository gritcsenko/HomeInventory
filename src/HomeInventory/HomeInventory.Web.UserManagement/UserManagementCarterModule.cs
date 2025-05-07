using AutoMapper;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Reactive.Disposables;
using HomeInventory.Application.Framework.Messaging;

namespace HomeInventory.Web.UserManagement;

public class UserManagementCarterModule(IMapper mapper, ISender sender, IScopeAccessor scopeAccessor, IProblemDetailsFactory problemDetailsFactory) : ApiCarterModule("/api/users/manage")
{
    private readonly IMapper _mapper = mapper;
    private readonly ISender _sender = sender;
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    protected override void AddRoutes(RouteGroupBuilder group) =>
        group.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .WithValidationOf<RegisterRequest>();

    public async Task<Results<Ok<RegisterResponse>, ProblemHttpResult>> RegisterAsync([FromBody] RegisterRequest body, [FromServices] IUserRepository userRepository, [FromServices] IUnitOfWork unitOfWork, HttpContext context, CancellationToken cancellationToken = default)
    {
        using var scopes = new CompositeDisposable(
            _scopeAccessor.GetScope<IUserRepository>().Set(userRepository),
            _scopeAccessor.GetScope<IUnitOfWork>().Set(unitOfWork));

        var command = _mapper.MapOrFail<RegisterCommand>(body);
        var result = await _sender.Send(command, cancellationToken);
        return await result.Match<Task<Results<Ok<RegisterResponse>, ProblemHttpResult>>>(
            async error =>
            {
                var problem = _problemDetailsFactory.ConvertToProblem([error], context.TraceIdentifier);
                return await Task.FromResult(TypedResults.Problem(problem));
            },
            async () =>
            {
                var query = _mapper.MapOrFail<UserIdQuery>(body);
                var queryResult = await _sender.Send(query, cancellationToken);
                return _problemDetailsFactory.MatchToOk(queryResult, _mapper.MapOrFail<RegisterResponse>, context.TraceIdentifier);
            });
    }
}
