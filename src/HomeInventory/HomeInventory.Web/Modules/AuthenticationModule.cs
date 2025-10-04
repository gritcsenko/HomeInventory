using System.Reactive.Disposables;
using AutoMapper;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
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

public class AuthenticationModule(IMapper mapper, IProblemDetailsFactory problemDetailsFactory, IScopeAccessor scopeAccessor) : ApiCarterModule("/api/authentication")
{
    private readonly IMapper _mapper = mapper;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    protected override void AddRoutes(RouteGroupBuilder group) =>
        group.MapPost("login", LoginAsync)
            .AllowAnonymous()
            .WithValidationOf<LoginRequest>(static s => s.IncludeAllRuleSets());

    public async Task<Results<Ok<LoginResponse>, ProblemHttpResult>> LoginAsync([FromBody] LoginRequest body, [FromServices] IUserService userService, [FromServices] IUserRepository userRepository, [FromServices] IUnitOfWork unitOfWork, HttpContext context, CancellationToken cancellationToken = default)
    {
        using var scopes = new CompositeDisposable(
            _scopeAccessor.GetScope<IUserRepository>().Set(userRepository),
            _scopeAccessor.GetScope<IUnitOfWork>().Set(unitOfWork));
        
        var query = _mapper.MapOrFail<AuthenticateQuery>(body);
        var result = await userService.AuthenticateAsync(query, cancellationToken);
        return _problemDetailsFactory.MatchToOk(result, _mapper.MapOrFail<LoginResponse>, context.TraceIdentifier);
    }
}
