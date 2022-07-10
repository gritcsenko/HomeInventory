using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Application.Services.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<AuthenticateResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.FindByEmailAsync(email, cancellationToken) is not User user)
        {
            throw new Exception("DoesNoExist");
        }

        if (user.Password != password)
        {
            throw new Exception("Invalid password");
        }

        var token = await _tokenGenerator.GenerateTokenAsync(user, cancellationToken);
        return new AuthenticateResult(user.Id, token);
    }

    public async Task<RegistrationResult> RegisterAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.FindByEmailAsync(email, cancellationToken) is not null)
        {
            throw new Exception("AlreadyExist");
        }

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password,
        };
        await _userRepository.AddUserAsync(user, cancellationToken);

        return new RegistrationResult(user.Id);
    }
}
