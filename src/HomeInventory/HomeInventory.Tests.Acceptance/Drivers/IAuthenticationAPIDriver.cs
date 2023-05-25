using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IAuthenticationAPIDriver
{
    Task<LoginResponse> LoginAsync(LoginRequest requestBody);
}
