using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Errors;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Application.UserManagement.Commands;

internal sealed class RegisterCommandHandler(IScopeAccessor scopeAccessor, TimeProvider timeProvider, IPasswordHasher hasher, IIdSupplier<Ulid> eventIdSupplier) : CommandHandler<RegisterCommand>
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IPasswordHasher _hasher = hasher;
    private readonly IIdSupplier<Ulid> _eventIdSupplier = eventIdSupplier;

    protected override async Task<Option<Error>> InternalHandle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var userRepository = _scopeAccessor.GetRequiredContext<IUserRepository>();

        if (await userRepository.IsUserHasEmailAsync(command.Email, cancellationToken))
        {
            return new DuplicateEmailError();
        }

        var userId = UserId.IdSupplier.Supply();
        var user = await command.CreateUserAsync(userId, _hasher, cancellationToken);
        var result = await user
            .MapAsync(async u =>
            {
                u.OnUserCreated(_eventIdSupplier, _timeProvider);
                await userRepository.AddAsync(u, cancellationToken);
                return Option<Error>.None;
            });

        return result.IfFail(errors => Error.Many(errors));
    }
}

file static class Extensions
{
    public static async Task<Validation<Error, User>> CreateUserAsync(this RegisterCommand command, Ulid userId, IPasswordHasher hasher, CancellationToken cancellationToken = default) =>
        await UserId
            .CreateBuilder()
            .WithValue(userId)
            .Build()
            .MapAsync(async id => await CreateUserAsync(command, hasher, id, cancellationToken));

    private static async Task<User> CreateUserAsync(RegisterCommand command, IPasswordHasher hasher, UserId id, CancellationToken cancellationToken)
    {
        var password = await hasher.HashAsync(command.Password, cancellationToken);
        var user = new User(id)
        {
            Email = command.Email,
            Password = password,
        };
        return user;
    }
}
