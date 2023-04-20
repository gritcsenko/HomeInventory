using System.Net.Http.Json;
using Microsoft.AspNetCore.TestHost;
using Throw;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal abstract class ApiDriver
{
    private readonly TestServer _server;
    private readonly string _basePath;

    protected ApiDriver(TestServer server, string basePath)
    {
        _server = server;
        _basePath = basePath;
    }

    protected async Task<TResponse> GetAsync<TResponse>(string path)
        where TResponse : class
    {
        var result = await _server.CreateRequest(_basePath + path)
          .GetAsync();

        result.EnsureSuccessStatusCode();

        var body = await result.Content.ReadFromJsonAsync<TResponse>();
        return body.ThrowIfNull().Value;
    }
}
