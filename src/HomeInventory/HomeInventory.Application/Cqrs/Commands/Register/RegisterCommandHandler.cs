using FluentResults;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal class RegisterCommandHandler : CommandHandler<RegisterCommand, RegistrationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdFactory<UserId> _userIdFactory;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IUserRepository userRepository, IIdFactory<UserId> userIdFactory, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userIdFactory = userIdFactory;
        _unitOfWork = unitOfWork;
    }

    protected override async Task<Result<RegistrationResult>> InternalHandle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await IsUserHasEmailAsync(command, cancellationToken))
        {
            return new DuplicateEmailError();
        }

        var user = await CreateUserAsync(command, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new RegistrationResult(user.Id);
    }

    private async Task<bool> IsUserHasEmailAsync(RegisterCommand request, CancellationToken cancellationToken) =>
        await _userRepository.IsUserHasEmailAsync(request.Email, cancellationToken);

    private async Task<User> CreateUserAsync(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userId = _userIdFactory.CreateNew();
        var user = new User(userId)
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
        };
        await _userRepository.AddAsync(user, cancellationToken);
        return user;
    }
}
