using System.Net.Http.Json;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;
using Throw;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class UserManagementAPIDriver : IUserManagementAPIDriver
{
    private readonly TestServer _server;
    private readonly string _basePath;

    public UserManagementAPIDriver(TestServer server)
        : this(server, "/api/users/manage")
    {
    }

    public UserManagementAPIDriver(TestServer server, string basePath)
    {
        _server = server;
        _basePath = basePath;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest requestBody)
    {
        var result = await _server.CreateRequest(_basePath + "/register")
            .And(m => m.Content = JsonContent.Create(requestBody))
            .PostAsync();
        if (result.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            return new RegisterResponse(Guid.NewGuid());
        }

        result.EnsureSuccessStatusCode();

        var body = await result.Content.ReadFromJsonAsync<RegisterResponse>();
        return body.ThrowIfNull().Value;
    }
}
