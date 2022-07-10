using HomeInventory.Application.Interfaces.Authentication;

namespace HomeInventory.Application.Services.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator;

    public AuthenticationService(IAuthenticationTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthenticateResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var userId = Guid.NewGuid();
        var token = await _tokenGenerator.GenerateTokenAsync(userId, cancellationToken);
        return new AuthenticateResult(userId, token);
    }

    public async Task<RegistrationResult> RegisterAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken = default)
    {
        return new RegistrationResult(Guid.NewGuid());
    }
}
