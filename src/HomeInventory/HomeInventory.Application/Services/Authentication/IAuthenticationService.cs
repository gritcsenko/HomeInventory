namespace HomeInventory.Application.Services.Authentication;

public interface IAuthenticationService
{
    Task<RegistrationResult> RegisterAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken = default);

    Task<AuthenticateResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}

public record class RegistrationResult(Guid Id);

public record class AuthenticateResult(Guid Id, string Token);
