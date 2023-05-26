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

    protected async Task<TResponse> GetAsync<TResponse>()
        where TResponse : class =>
        await GetAsync<TResponse>(string.Empty);

    protected async Task<TResponse> GetAsync<TResponse>(string path)
        where TResponse : class
    {
        var result = await _server.CreateRequest(_basePath + path)
          .GetAsync();

        result.EnsureSuccessStatusCode();

        var body = await result.Content.ReadFromJsonAsync<TResponse>();
        return body.ThrowIfNull().Value;
    }


    protected async ValueTask<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest requestBody)
        where TResponse : class
    {
        var result = await _server
            .CreateRequest(_basePath + path)
            .And(m => m.Content = JsonContent.Create(requestBody))
            .PostAsync();

        result.EnsureSuccessStatusCode();

        var body = await result.Content.ReadFromJsonAsync<TResponse>();
        return body.ThrowIfNull().Value;
    }
}
