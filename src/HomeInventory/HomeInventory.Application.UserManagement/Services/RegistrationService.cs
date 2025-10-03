using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Errors;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Application.UserManagement.Services;

internal sealed class RegistrationService(
    IUserRepository userRepository,
    TimeProvider timeProvider,
    IPasswordHasher hasher,
    IIdSupplier<Ulid> eventIdSupplier) : IRegistrationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IPasswordHasher _hasher = hasher;
    private readonly IIdSupplier<Ulid> _eventIdSupplier = eventIdSupplier;

    public async Task<Option<Error>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.IsUserHasEmailAsync(command.Email, cancellationToken))
        {
            return DuplicateEmailError.Instance;
        }

        var userId = UserId.Supplier.SupplyNew();
        var user = await command.CreateUserAsync(userId, _hasher, cancellationToken);
        var result = await user
            .MapAsync(async u =>
            {
                u.OnUserCreated(_eventIdSupplier, _timeProvider);
                await _userRepository.AddAsync(u, cancellationToken);
                return Option<Error>.None;
            });

        return result.IfFail(errors => Error.Many(errors));
    }
}

file static class RegistrationServiceExtensions
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
