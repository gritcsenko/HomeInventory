using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Core;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal class RegisterCommandHandler(IUserRepository userRepository, IDateTimeService dateTimeService, IPasswordHasher hasher) : CommandHandler<RegisterCommand>
{
    protected override async Task<OneOf<Success, IError>> InternalHandle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await userRepository.IsUserHasEmailAsync(command.Email, cancellationToken))
        {
            return new DuplicateEmailError();
        }

        var user = await command.CreateUserAsync(hasher, cancellationToken);
        var result = await user
            .Tap(u => u.OnUserCreated(dateTimeService))
            .Tap(u => userRepository.AddAsync(u, cancellationToken));

        return result
            .Convert<OneOf<Success, IError>>(_ => new Success())
            .OrInvoke(() => new ObjectValidationError<Ulid>(command.UserIdSupplier));
    }
}
