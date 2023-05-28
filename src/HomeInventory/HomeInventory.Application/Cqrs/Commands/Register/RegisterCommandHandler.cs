using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal class RegisterCommandHandler : CommandHandler<RegisterCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository) => _userRepository = userRepository;

    protected override async Task<OneOf<Success, IError>> InternalHandle(RegisterCommand command, CancellationToken cancellationToken)
    {
        await using var unit = await _userRepository.WithUnitOfWorkAsync(cancellationToken);
        if (await _userRepository.IsUserHasEmailAsync(command.Email, cancellationToken))
        {
            return new DuplicateEmailError();
        }

#pragma warning disable CA2252 // This API requires opting into preview features
        var user = command.CreateUser();
#pragma warning restore CA2252 // This API requires opting into preview features

        await _userRepository.AddAsync(user, cancellationToken);
        await unit.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}
