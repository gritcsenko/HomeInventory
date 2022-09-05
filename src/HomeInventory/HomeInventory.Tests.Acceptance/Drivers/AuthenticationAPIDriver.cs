using System.Net.Http.Json;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;
using Throw;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class AuthenticationAPIDriver : IAuthenticationAPIDriver
{
    private const string ControllerPath = "/api/Authentication";
    private TestServer _server;

    public AuthenticationAPIDriver(TestServer server)
    {
        _server = server;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest requestBody)
    {
        var result = await _server.CreateRequest(ControllerPath + "/register")
            .And(m => m.Content = JsonContent.Create(requestBody))
            .PostAsync();

        result.EnsureSuccessStatusCode();

        var body = await result.Content.ReadFromJsonAsync<RegisterResponse>();
        return body.ThrowIfNull().Value;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest requestBody)
    {
        var result = await _server.CreateRequest(ControllerPath + "/login")
            .And(m => m.Content = JsonContent.Create(requestBody))
            .PostAsync();

        result.EnsureSuccessStatusCode();

        var body = await result.Content.ReadFromJsonAsync<LoginResponse>();
        return body.ThrowIfNull().Value;
    }
}
