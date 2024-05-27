using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Core;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Cqrs.Queries.UserId;

internal sealed class UserIdQueryHandler(IScopeAccessor scopeAccessor) : QueryHandler<UserIdQuery, UserIdResult>
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    protected override async Task<OneOf<UserIdResult, IError>> InternalHandle(UserIdQuery query, CancellationToken cancellationToken)
    {
        var userRepository = _scopeAccessor.Get<IUserRepository>().OrThrow<InvalidOperationException>();
        var result = await userRepository.FindFirstByEmailUserOptionalAsync(query.Email, cancellationToken);
        return result
            .Convert<OneOf<UserIdResult, IError>>(user => new UserIdResult(user.Id))
            .OrInvoke(() => new NotFoundError($"User with email {query.Email} was not found"));
    }
}
