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
using HomeInventory.Application.UserManagement.Interfaces;

namespace HomeInventory.Web.UserManagement;

public class UserManagementCarterModule(IScopeAccessor scopeAccessor, IProblemDetailsFactory problemDetailsFactory, ContractsMapper mapper) : ApiCarterModule
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;
    private readonly ContractsMapper _mapper = mapper;

    protected override string PathPrefix => "/api/users/manage";

    protected override void AddRoutes(RouteGroupBuilder group) =>
        group.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .WithValidationOf<RegisterRequest>();

    public async Task<Results<Ok<RegisterResponse>, ProblemHttpResult>> RegisterAsync([FromBody] RegisterRequest body, [FromServices] IUserService userService, [FromServices] IUserRepository userRepository, [FromServices] IUnitOfWork unitOfWork, HttpContext context, CancellationToken cancellationToken = default)
    {
        using var scopes = new CompositeDisposable(
            _scopeAccessor.GetScope<IUserRepository>().Set(userRepository),
            _scopeAccessor.GetScope<IUnitOfWork>().Set(unitOfWork));

        var command = _mapper.ToCommand(body);
        var result = await userService.RegisterAsync(command, cancellationToken);
        return await result.Match<Task<Results<Ok<RegisterResponse>, ProblemHttpResult>>>(
            async error =>
            {
                var problem = _problemDetailsFactory.ConvertToProblem([error], context.TraceIdentifier);
                return await Task.FromResult(TypedResults.Problem(problem));
            },
            async () =>
            {
                var query = _mapper.ToQuery(body);
                var queryResult = await userService.GetUserIdAsync(query, cancellationToken);
                return _problemDetailsFactory.MatchToOk(queryResult, _mapper.ToResponse, context.TraceIdentifier);
            });
    }
}
