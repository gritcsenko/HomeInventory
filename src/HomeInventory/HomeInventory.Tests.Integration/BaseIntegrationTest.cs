using System.Net.Http.Json;
using HomeInventory.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace HomeInventory.Tests.Integration;

[IntegrationTest]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "See InitializeDisposables")]
public abstract class BaseIntegrationTest : BaseTest
{
    private readonly WebApplicationFactory<Api.AppBuilder> _appFactory = new();
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _testOutputHelper;

    protected BaseIntegrationTest(ITestOutputHelper testOutputHelper)
    {
        _client = _appFactory.CreateClient();
        _testOutputHelper = testOutputHelper;

        AddDisposable(_client);
        AddAsyncDisposable(_appFactory);
    }

    protected IEnumerable<RouteEndpoint> GetEndpoints() =>
        _appFactory
            .Services
            .GetServices<EndpointDataSource>()
            .SelectMany(static s => s.Endpoints)
            .OfType<RouteEndpoint>();

    protected async Task<HttpResponseMessage> PostAsync(string route, JsonContent content)
    {
        var requestUri = new Uri(route, UriKind.RelativeOrAbsolute);
        _testOutputHelper.WriteLine($"Sending POST to {requestUri}...");
        return await _client.PostAsync(requestUri, content, Cancellation.Token);
    }
}
