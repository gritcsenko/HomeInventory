using System.Runtime.Versioning;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal class RegisterCommandHandler : CommandHandler<RegisterCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeService _dateTimeService;

    public RegisterCommandHandler(IUserRepository userRepository, IDateTimeService dateTimeService)
    {
        _userRepository = userRepository;
        _dateTimeService = dateTimeService;
    }

    [RequiresPreviewFeatures]
    protected override async Task<OneOf<Success, IError>> InternalHandle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.IsUserHasEmailAsync(command.Email, cancellationToken))
        {
            return new DuplicateEmailError();
        }

        var user = command.CreateUser(_dateTimeService);

        await _userRepository.AddAsync(user, cancellationToken);

        return new Success();
    }
}
