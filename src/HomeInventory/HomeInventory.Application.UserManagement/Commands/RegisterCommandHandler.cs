using DotNext;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Core;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using Visus.Cuid;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal sealed class RegisterCommandHandler(IScopeAccessor scopeAccessor, TimeProvider timeProvider, IPasswordHasher hasher, ISupplier<Cuid> eventIdSupplier) : CommandHandler<RegisterCommand>
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IPasswordHasher _hasher = hasher;
    private readonly ISupplier<Cuid> _eventIdSupplier = eventIdSupplier;

    protected override async Task<OneOf<Success, IError>> InternalHandle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var userRepository = _scopeAccessor.Get<IUserRepository>().OrThrow<InvalidOperationException>();
        if (await userRepository.IsUserHasEmailAsync(command.Email, cancellationToken))
        {
            return new DuplicateEmailError();
        }

        var user = await command.CreateUserAsync(_hasher, cancellationToken);
        var result = await user
            .Tap(u => u.OnUserCreated(_eventIdSupplier, _timeProvider))
            .Tap(u => userRepository.AddAsync(u, cancellationToken));

        return result
            .Convert<OneOf<Success, IError>>(_ => new Success())
            .OrInvoke(() => UserId.IdSupplier.CreateObjectValidationError());
    }
}

file static class SupplierExtensions
{
    public static OneOf<Success, IError> CreateObjectValidationError<TId>(this ISupplier<TId> supplier) => new ObjectValidationError<TId>(supplier);
}
