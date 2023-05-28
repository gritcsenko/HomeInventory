using System.Runtime.Versioning;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal static class RegisterCommandExtensions
{
    [RequiresPreviewFeatures]
    public static User CreateUser(this RegisterCommand command) =>
        new(UserId
            .CreateBuilder()
            .WithValue(command.UserIdSupplier)
            .Invoke())
        {
            Email = command.Email,
            Password = command.Password,
        };
}
