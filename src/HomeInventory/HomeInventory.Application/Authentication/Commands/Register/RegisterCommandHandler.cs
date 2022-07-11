using ErrorOr;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Entities;
using MediatR;

namespace HomeInventory.Application.Authentication.Commands.Register;
internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<RegistrationResult>>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<RegistrationResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.FindByEmailAsync(request.Email, cancellationToken) is not null)
        {
            return Domain.Errors.User.DuplicateEmail;
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
        };
        await _userRepository.AddUserAsync(user, cancellationToken);

        return new RegistrationResult(user.Id);
    }
}
