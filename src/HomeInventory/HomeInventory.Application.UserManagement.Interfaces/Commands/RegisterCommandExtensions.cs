using DotNext;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Core;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

public static class RegisterCommandExtensions
{
    public static async ValueTask<Optional<User>> CreateUserAsync(this RegisterCommand command, IPasswordHasher hasher, CancellationToken cancellationToken = default) =>
        await UserId
            .CreateBuilder()
            .WithValue(command.UserIdSupplier.Invoke())
            .Invoke()
            .ConvertAsync(async (id, t) =>
            {
                var password = await hasher.HashAsync(command.Password, t);
                return new User(id)
                {
                    Email = command.Email,
                    Password = password,
                };
            }, cancellationToken);
}
