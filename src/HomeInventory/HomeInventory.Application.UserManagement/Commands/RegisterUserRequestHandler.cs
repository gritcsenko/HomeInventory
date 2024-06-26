using DotNext;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Core;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal sealed class RegisterUserRequestHandler(IScopeAccessor scopeAccessor, IPasswordHasher hasher) : IRequestHandler<RegisterUserRequestMessage, Success>
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly IPasswordHasher _hasher = hasher;

    public async Task<OneOf<Success, IError>> HandleAsync(IMessageHub hub, RegisterUserRequestMessage request, CancellationToken cancellationToken = default)
    {
        var userRepository = _scopeAccessor.TryGet<IUserRepository>().OrThrow<InvalidOperationException>();
        if (await userRepository.IsUserHasEmailAsync(request.Email, cancellationToken))
        {
            return new DuplicateEmailError();
        }

        var userIdSupplier = UserId.IdSupplier;
        var userId = UserId
            .CreateBuilder()
            .WithValue(userIdSupplier.Invoke())
            .Build();
        var user = await request.CreateUserAsync(userId, _hasher, cancellationToken)
            .Tap(u => u.OnUserCreated(hub.EventIdSupplier, hub.EventCreatedTimeProvider))
            .Tap(u => userRepository.AddAsync(u, cancellationToken));
        return user.ToResultOrError(_ => new Success(), () => userId.CreateObjectValidationError().OrInvoke(() => new ObjectValidationError<string>("Failed to create new id")));
    }
}


file static class Extensions
{
    public static Optional<IError> CreateObjectValidationError<TId>(this Optional<TId> optional) =>
        from id in optional
        select (IError)new ObjectValidationError<TId>(id);

    public static Task<Optional<User>> CreateUserAsync(this RegisterUserRequestMessage request, Optional<UserId> userId, IPasswordHasher hasher, CancellationToken cancellationToken = default) =>
        userId.CreateUserAsync(request.Email, request.Password, hasher, cancellationToken);

    private static Task<Optional<User>> CreateUserAsync(this Optional<UserId> userId, Email email, string password, IPasswordHasher hasher, CancellationToken cancellationToken = default)
    {
        return
            from id in userId
            let hash = hasher.HashAsync(password, cancellationToken)
            select CreateUserAsync(id, hash);

        async Task<User> CreateUserAsync(UserId id, Task<string> hash) =>
            new User(id)
            {
                Email = email,
                Password = await hash,
            };
    }
}

