using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Cqrs.Queries.UserId;

internal sealed class UserIdQueryMessageHandler(IScopeAccessor scopeAccessor) : QueryHandler<UserIdQueryMessage, UserIdResult>
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    protected override async Task<Validation<Error, UserIdResult>> InternalHandle(IRequestContext<UserIdQueryMessage> context)
    {
        var userRepository = _scopeAccessor.GetRequiredContext<IUserRepository>();
        var result = await userRepository.FindFirstByEmailUserOptionalAsync(context.Request.Email, context.RequestAborted);
        return result
            .Map(user => (Validation<Error, UserIdResult>)new UserIdResult(user.Id))
            .ErrorIfNone(() => new NotFoundError($"User with email {context.Request.Email} was not found"));
    }
}
