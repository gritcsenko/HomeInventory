using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

public class AuthenticationModule : ApiCarterModule
{
    protected override string PathPrefix => "/api/authentication";

    protected override void AddRoutes(RouteGroupBuilder group) =>
        group.MapPost("login", LoginAsync)
            .AllowAnonymous()
            .WithValidationOf<LoginRequest>(static s => s.IncludeAllRuleSets());

    public static async Task<Results<Ok<LoginResponse>, ProblemHttpResult>> LoginAsync([FromBody] LoginRequest body, [FromServices] IUserService userService, [FromServices] IUserRepository userRepository, [FromServices] IUnitOfWork unitOfWork, HttpContext context, [FromServices] IProblemDetailsFactory problemDetailsFactory, [FromServices] IScopeAccessor scopeAccessor, [FromServices] ContractsMapper mapper, CancellationToken cancellationToken = default)
    {
        using var scopes = new CompositeDisposable(
            scopeAccessor.GetScope<IUserRepository>().Set(userRepository),
            scopeAccessor.GetScope<IUnitOfWork>().Set(unitOfWork));

        var query = mapper.ToQuery(body);
        var result = await userService.AuthenticateAsync(query, cancellationToken);
        return problemDetailsFactory.MatchToOk(result, mapper.ToResponse, context.TraceIdentifier);
    }
}
