using AutoMapper;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Core;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeInventory.Web.Modules;

public class AuthenticationModule(IScopeAccessor scopeAccessor) : ApiModule("/api/authentication")
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapPost("login", LoginAsync)
            .AllowAnonymous()
            .WithValidationOf<LoginRequest>(s => s.IncludeAllRuleSets());
    }

    public async Task<Results<Ok<LoginResponse>, ProblemHttpResult>> LoginAsync([FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var mapper = _scopeAccessor.TryGet<IMapper>().OrThrow<InvalidOperationException>();
        var hub = _scopeAccessor.TryGet<IMessageHub>().OrThrow<InvalidOperationException>();
        var factory = _scopeAccessor.TryGet<IProblemDetailsFactory>().OrThrow<InvalidOperationException>();

        var request = mapper.MapOrFail<AuthenticateRequestMessage>(body, o => o.State = hub);
        var response = await hub.RequestAsync(request, cancellationToken);
        return factory.MatchToOk(response, mapper.MapOrFail<LoginResponse>);
    }
}
