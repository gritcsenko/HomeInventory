using System.Runtime.Versioning;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal static class RegisterCommandExtensions
{
    [RequiresPreviewFeatures]
    public static User CreateUser(this RegisterCommand command, IDateTimeService dateTimeService)
    {
        var userId = UserId
            .CreateBuilder()
            .WithValue(command.UserIdSupplier)
            .Invoke();
        var user = new User(userId)
        {
            Email = command.Email,
            Password = command.Password,
        };
        user.OnUserCreated(dateTimeService);
        return user;
    }
}
