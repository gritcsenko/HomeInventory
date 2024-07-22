using AutoMapper;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Core;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Infrastructure;
using Microsoft.AspNetCore.Builder;
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

    public async Task<Results<Ok<RegisterResponse>, ProblemHttpResult>> RegisterAsync([FromBody] RegisterRequest body, [FromServices] IUserRepository userRepository, CancellationToken cancellationToken = default)
    {
        using var _ = _scopeAccessor.Set(userRepository);
        var mapper = _scopeAccessor.TryGet<IMapper>().OrThrow<InvalidOperationException>();
        var hub = _scopeAccessor.TryGet<IMessageHub>().OrThrow<InvalidOperationException>();
        var factory = _scopeAccessor.TryGet<IProblemDetailsFactory>().OrThrow<InvalidOperationException>();
        var command = mapper.MapOrFail<RegisterUserRequestMessage>(body, o => o.State = hub);
        var response = await hub.RequestAsync(command, cancellationToken);

        var result = await factory.MatchToOk(response,
            async success =>
            {
                var query = mapper.MapOrFail<UserIdQueryMessage>(body, o => o.State = hub);
                var queryResult = await hub.RequestAsync(query, cancellationToken);
                return factory.MatchToOk(queryResult, mapper.MapOrFail<RegisterResponse>);
            });
        return result.Unwrap();
    }
}
