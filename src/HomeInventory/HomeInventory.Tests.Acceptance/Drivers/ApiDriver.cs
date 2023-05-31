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

    protected async ValueTask<TResponse> GetAsync<TResponse>(string path)
        where TResponse : class
    {
        var result = await _server.CreateRequest(_basePath + path)
          .GetAsync();

        await EnsureSuccessStatusCodeAsync(result);

        return await ReadBodyAsync<TResponse>(result);
    }


    protected async ValueTask<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest requestBody)
        where TResponse : class
    {
        var result = await PostAsync(path, requestBody);

        await EnsureSuccessStatusCodeAsync(result);

        return await ReadBodyAsync<TResponse>(result);
    }

    protected async ValueTask<HttpResponseMessage> PostAsync<TRequest>(string path, TRequest requestBody)
    {
        return await _server
            .CreateRequest(_basePath + path)
            .And(m => m.Content = JsonContent.Create(requestBody))
            .PostAsync();
    }

    protected static async ValueTask EnsureSuccessStatusCodeAsync(HttpResponseMessage result)
    {
        if (!result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Response status code does not indicate success: {(int)result.StatusCode} ({result.ReasonPhrase}). Content: {content}");
        }
    }

    protected static async ValueTask<TResponse> ReadBodyAsync<TResponse>(HttpResponseMessage result) where TResponse : class
    {
        var body = await result.Content.ReadFromJsonAsync<TResponse>();
        return body.ThrowIfNull().Value;
    }
}
