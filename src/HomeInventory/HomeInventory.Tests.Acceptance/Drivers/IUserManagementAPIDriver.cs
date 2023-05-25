using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal interface IUserManagementAPIDriver
{
    ValueTask<RegisterResponse> RegisterAsync(RegisterRequest requestBody);
}
