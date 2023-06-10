using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal sealed class UserManagementApiDriver : ApiDriver, IUserManagementApiDriver
{
    public UserManagementApiDriver(TestServer server)
        : base(server, "/api/users/manage")
    {
    }

    public async ValueTask<RegisterResponse> RegisterAsync(RegisterRequest requestBody) =>
        await PostAsync<RegisterRequest, RegisterResponse>("/register", requestBody);
}
