using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IAuthenticationAPIDriver
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest requestBody);

    Task<LoginResponse> LoginAsync(LoginRequest requestBody);
}
