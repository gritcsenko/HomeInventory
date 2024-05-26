using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class AuthenticationApiDriver(TestServer server) : ApiDriver(server, "/api/Authentication"), IAuthenticationApiDriver
{
    public async ValueTask<LoginResponse> LoginAsync(LoginRequest requestBody)
        => await CreatePostRequest("/login")
            .WithJsonBody(requestBody)
            .SendAsync<LoginResponse>();
}
