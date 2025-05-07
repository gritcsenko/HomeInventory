using Flurl;
using HomeInventory.Contracts.UserManagement;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal sealed class UserManagementApiDriver(TestServer server) : ApiDriver(server, string.Empty.AppendPathSegments("api", "users", "manage")), IUserManagementApiDriver
{
    public async ValueTask<RegisterResponse> RegisterAsync(RegisterRequest requestBody) =>
        await CreatePostRequest("register")
            .WithJsonBody(requestBody)
            .SendAsync<RegisterResponse>();
}
