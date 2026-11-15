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

public class UserManagementCarterModule : ApiCarterModule
{
    protected override string PathPrefix => "/api/users/manage";

    protected override void AddRoutes(RouteGroupBuilder group) =>
        group.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .WithValidationOf<RegisterRequest>();

    public static async Task<Results<Ok<RegisterResponse>, ProblemHttpResult>> RegisterAsync([FromBody] RegisterRequest body, [FromServices] IUserService userService, [FromServices] IUserRepository userRepository, [FromServices] IUnitOfWork unitOfWork, HttpContext context, [FromServices] IScopeAccessor scopeAccessor, [FromServices] IProblemDetailsFactory problemDetailsFactory, [FromServices] ContractsMapper mapper, CancellationToken cancellationToken = default)
    {
        using var scopes = new CompositeDisposable(
            scopeAccessor.GetScope<IUserRepository>().Set(userRepository),
            scopeAccessor.GetScope<IUnitOfWork>().Set(unitOfWork));

        var command = mapper.ToCommand(body);
        var result = await userService.RegisterAsync(command, cancellationToken);
        return await result.Match<Task<Results<Ok<RegisterResponse>, ProblemHttpResult>>>(
            async error =>
            {
                var problem = problemDetailsFactory.ConvertToProblem([error], context.TraceIdentifier);
                return await Task.FromResult(TypedResults.Problem(problem));
            },
            async () =>
            {
                var query = mapper.ToQuery(body);
                var queryResult = await userService.GetUserIdAsync(query, cancellationToken);
                return problemDetailsFactory.MatchToOk(queryResult, mapper.ToResponse, context.TraceIdentifier);
            });
    }
}
