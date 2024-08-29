using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal sealed class RegisterUserRequestHandler(IScopeAccessor scopeAccessor, IPasswordHasher hasher, IIdSupplier<Ulid> eventIdSupplier, TimeProvider timeProvider) : IRequestHandler<RegisterUserRequestMessage, Option<Error>>
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly IPasswordHasher _hasher = hasher;
    private readonly IIdSupplier<Ulid> _eventIdSupplier = eventIdSupplier;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Option<Error>> HandleAsync(IRequestContext<RegisterUserRequestMessage> context)
    {
        var userRepository = _scopeAccessor.GetRequiredContext<IUserRepository>();
        if (await userRepository.IsUserHasEmailAsync(context.Request.Email, context.RequestAborted))
        {
            return new DuplicateEmailError();
        }

        var userId = UserId.IdSupplier.Supply();
        var user = await context.Request.CreateUserAsync(userId, _hasher, context.RequestAborted);
        var result = await user
            .MapAsync(async u =>
            {
                u.OnUserCreated(_eventIdSupplier, _timeProvider);
                await userRepository.AddAsync(u, context.RequestAborted);
                return Option<Error>.None;
            });
        return result.IfFail(errors => Error.Many(errors));
    }
}

file static class Extensions
{
    public static async Task<Validation<Error, User>> CreateUserAsync(this RegisterUserRequestMessage request, Ulid userId, IPasswordHasher hasher, CancellationToken cancellationToken = default) =>
        await UserId
            .CreateBuilder()
            .WithValue(userId)
            .Build()
            .MapAsync(async id => await CreateUserAsync(request, hasher, id, cancellationToken));

    private static async Task<User> CreateUserAsync(RegisterUserRequestMessage request, IPasswordHasher hasher, UserId id, CancellationToken cancellationToken)
    {
        var password = await hasher.HashAsync(request.Password, cancellationToken);
        var user = new User(id)
        {
            Email = request.Email,
            Password = password,
        };
        return user;
    }
}
