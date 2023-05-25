using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal sealed class UserManagementAPIDriver : ApiDriver, IUserManagementAPIDriver
{
    public UserManagementAPIDriver(TestServer server)
        : base(server, "/api/users/manage")
    {
    }

    public async ValueTask<RegisterResponse> RegisterAsync(RegisterRequest requestBody) =>
        await PostAsync<RegisterRequest, RegisterResponse>("/register", requestBody);
}
