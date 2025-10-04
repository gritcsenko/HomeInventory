using HomeInventory.Contracts.UserManagement;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal interface IUserManagementApiDriver
{
    ValueTask<RegisterResponse> RegisterAsync(RegisterRequest requestBody);
}
