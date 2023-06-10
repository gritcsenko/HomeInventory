using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IAuthenticationApiDriver
{
    Task<LoginResponse> LoginAsync(LoginRequest requestBody);
}
