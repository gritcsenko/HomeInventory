using System.Runtime.Versioning;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

public static class RegisterCommandExtensions
{
    [RequiresPreviewFeatures]
    public static async ValueTask<User> CreateUserAsync(this RegisterCommand command, IPasswordHasher hasher, CancellationToken cancellationToken = default)
    {
        var userId = UserId
            .CreateBuilder()
            .WithValue(command.UserIdSupplier)
            .Invoke();

        return new(userId)
        {
            Email = command.Email,
            Password = await hasher.HashAsync(command.Password, cancellationToken),
        };
    }
}
