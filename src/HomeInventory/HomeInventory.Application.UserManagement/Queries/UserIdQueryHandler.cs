using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.UserManagement.Persistence;

namespace HomeInventory.Application.UserManagement.Queries;

internal sealed class UserIdQueryHandler(IScopeAccessor scopeAccessor) : QueryHandler<UserIdQuery, UserIdResult>
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    protected override async Task<Validation<Error, UserIdResult>> InternalHandle(UserIdQuery query, CancellationToken cancellationToken)
    {
        var userRepository = _scopeAccessor.GetRequiredContext<IUserRepository>();

        var result = await userRepository.FindFirstByEmailUserOptionalAsync(query.Email, cancellationToken);
        return result
            .Map(user => (Validation<Error, UserIdResult>)new UserIdResult(user.Id))
            .ErrorIfNone(() => new NotFoundError($"User with email {query.Email} was not found"));
    }
}
