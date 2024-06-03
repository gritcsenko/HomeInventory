using HomeInventory.Core;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Cqrs.Queries.UserId;

internal sealed class UserIdQueryMessageHandler(IScopeAccessor scopeAccessor) : IRequestHandler<UserIdQueryMessage, UserIdResult>
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task<OneOf<UserIdResult, IError>> HandleAsync(IMessageHub hub, UserIdQueryMessage request, CancellationToken cancellationToken = default)
    {
        var userRepository = _scopeAccessor.TryGet<IUserRepository>().OrThrow<InvalidOperationException>();
        var result = await userRepository.FindFirstByEmailUserOptionalAsync(request.Email, cancellationToken);
        return result
            .Convert<OneOf<UserIdResult, IError>>(user => new UserIdResult(user.Id))
            .OrInvoke(() => new NotFoundError($"User with email {request.Email} was not found"));
    }
}
