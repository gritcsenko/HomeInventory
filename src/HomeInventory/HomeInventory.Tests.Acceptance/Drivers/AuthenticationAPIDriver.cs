using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class AuthenticationApiDriver : ApiDriver, IAuthenticationApiDriver
{
    public AuthenticationApiDriver(TestServer server)
        : base(server, "/api/Authentication")
    {
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest requestBody)
    {
        return await PostAsync<LoginRequest, LoginResponse>("/login", requestBody);
    }
}
