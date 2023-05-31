using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class AuthenticationAPIDriver : ApiDriver, IAuthenticationAPIDriver
{
    public AuthenticationAPIDriver(TestServer server)
        : base(server, "/api/Authentication")
    {
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest requestBody)
    {
        var result = await PostAsync("/register", requestBody);

        if (result.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            return new RegisterResponse(Guid.NewGuid());
        }

        await EnsureSuccessStatusCodeAsync(result);

        return await ReadBodyAsync<RegisterResponse>(result);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest requestBody)
    {
        return await PostAsync<LoginRequest, LoginResponse>("/login", requestBody);
    }
}
