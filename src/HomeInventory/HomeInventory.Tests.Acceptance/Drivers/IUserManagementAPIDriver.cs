using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal interface IUserManagementApiDriver
{
    ValueTask<RegisterResponse> RegisterAsync(RegisterRequest requestBody);
}
