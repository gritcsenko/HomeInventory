using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Core;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal class RegisterCommandHandler : CommandHandler<RegisterCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeService _dateTimeService;
    private readonly IPasswordHasher _hasher;

    public RegisterCommandHandler(IUserRepository userRepository, IDateTimeService dateTimeService, IPasswordHasher hasher)
    {
        _userRepository = userRepository;
        _dateTimeService = dateTimeService;
        _hasher = hasher;
    }

    protected override async Task<OneOf<Success, IError>> InternalHandle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.IsUserHasEmailAsync(command.Email, cancellationToken))
        {
            return new DuplicateEmailError();
        }

        var user = await command.CreateUserAsync(_hasher, cancellationToken);
        var result = await user
            .Tap(u => u.OnUserCreated(_dateTimeService))
            .Tap(u => _userRepository.AddAsync(u, cancellationToken));

        return result
            .Convert<OneOf<Success, IError>>(_ => new Success())
            .OrInvoke(() => new ObjectValidationError<Ulid>(command.UserIdSupplier));
    }
}
