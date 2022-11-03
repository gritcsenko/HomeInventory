using FluentResults;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Authentication.Commands.Register;
internal class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegistrationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdFactory<UserId> _userIdFactory;

    public RegisterCommandHandler(IUserRepository userRepository, IIdFactory<UserId> userIdFactory)
    {
        _userRepository = userRepository;
        _userIdFactory = userIdFactory;
    }

    public async Task<Result<RegistrationResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await IsUserHasEmailAsync(request, cancellationToken))
        {
            return Result.Fail<RegistrationResult>(new DuplicateEmailError());
        }

        var user = await CreateUserAsync(request, cancellationToken);
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
        return await _userRepository.AddAsync(user, cancellationToken);
    }
}
