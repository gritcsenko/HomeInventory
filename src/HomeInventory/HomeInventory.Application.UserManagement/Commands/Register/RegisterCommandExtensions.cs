using System.Runtime.Versioning;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal static class RegisterCommandExtensions
{
    [RequiresPreviewFeatures]
    public static async ValueTask<User> CreateUserAsync(this RegisterCommand command, IPasswordHasher _hasher, CancellationToken cancellationToken = default)
    {
        var userId = UserId
            .CreateBuilder()
            .WithValue(command.UserIdSupplier)
            .Invoke();

        return new(userId)
        {
            Email = command.Email,
            Password = await _hasher.HashAsync(command.Password, cancellationToken),
        };
    }
}
