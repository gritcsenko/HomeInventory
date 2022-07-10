namespace HomeInventory.Application.Services.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    public async Task<AuthenticateResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        return new AuthenticateResult(Guid.NewGuid(), "token");
    }

    public async Task<RegistrationResult> RegisterAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken = default)
    {
        return new RegistrationResult(Guid.NewGuid());
    }
}
