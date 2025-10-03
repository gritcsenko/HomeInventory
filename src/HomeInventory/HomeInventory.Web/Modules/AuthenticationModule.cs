using AutoMapper;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Contracts;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Framework.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

public class AuthenticationModule(IMapper mapper, IAuthenticationService authenticationService, IProblemDetailsFactory problemDetailsFactory) : ApiCarterModule("/api/authentication")
{
    private readonly IMapper _mapper = mapper;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;

    protected override void AddRoutes(RouteGroupBuilder group) =>
        group.MapPost("login", LoginAsync)
            .AllowAnonymous()
            .WithValidationOf<LoginRequest>(static s => s.IncludeAllRuleSets());

    public async Task<Results<Ok<LoginResponse>, ProblemHttpResult>> LoginAsync([FromBody] LoginRequest body, HttpContext context, CancellationToken cancellationToken = default)
    {
        var query = _mapper.MapOrFail<AuthenticateQuery>(body);
        var result = await _authenticationService.AuthenticateAsync(query, cancellationToken);
        return _problemDetailsFactory.MatchToOk(result, _mapper.MapOrFail<LoginResponse>, context.TraceIdentifier);
    }
}
