using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IAuthenticationApiDriver
{
    ValueTask<LoginResponse> LoginAsync(LoginRequest requestBody);
}
