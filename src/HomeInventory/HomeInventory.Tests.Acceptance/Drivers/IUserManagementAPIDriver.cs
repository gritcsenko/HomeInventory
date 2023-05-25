using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IUserManagementAPIDriver
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest requestBody);
}
