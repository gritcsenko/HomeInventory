using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class AuthenticationApiDriver : ApiDriver, IAuthenticationApiDriver
{
    public AuthenticationApiDriver(TestServer server)
        : base(server, "/api/Authentication")
    {
    }

    public async ValueTask<LoginResponse> LoginAsync(LoginRequest requestBody)
        => await CreatePostRequest("/login")
            .WithJsonBody(requestBody)
            .SendAsync<LoginResponse>();
}
