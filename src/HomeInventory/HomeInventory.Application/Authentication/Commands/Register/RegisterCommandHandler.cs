using ErrorOr;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using MediatR;

namespace HomeInventory.Application.Authentication.Commands.Register;
internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<RegistrationResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIdFactory _userIdFactory;

    public RegisterCommandHandler(IUserRepository userRepository, IUserIdFactory userIdFactory)
    {
        _userRepository = userRepository;
        _userIdFactory = userIdFactory;
    }

    public async Task<ErrorOr<RegistrationResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.HasEmailAsync(request.Email, cancellationToken)
            ? Errors.User.DuplicateEmail
            : await _userIdFactory.CreateNew().Match<Task<ErrorOr<RegistrationResult>>>(
                async userId => await AddUserAsync(request, userId, cancellationToken),
                async error => Errors.User.UserIdCreation);
    }

    private async Task<RegistrationResult> AddUserAsync(RegisterCommand request, UserId userId, CancellationToken cancellationToken)
    {
        var user = new User(userId)
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
        };
        await _userRepository.AddAsync(user, cancellationToken);

        return new RegistrationResult(userId);
    }
}
