using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace HomeInventory.Tests.Integration;

[IntegrationTest]
public abstract class BaseIntegrationTest : BaseTest
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly WebApplicationFactory<Program> _appFactory = new();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _testOutputHelper;

    protected BaseIntegrationTest(ITestOutputHelper testOutputHelper)
    {
        AddDisposable(_appFactory);
        _client = AddDisposable(_appFactory.CreateClient());
        _testOutputHelper = testOutputHelper;
    }

    protected IEnumerable<RouteEndpoint> GetEndpoints() =>
        _appFactory
            .Services
            .GetServices<EndpointDataSource>()
            .SelectMany(s => s.Endpoints)
            .OfType<RouteEndpoint>();

    protected async Task<HttpResponseMessage> PostAsync(string route, JsonContent content)
    {
        var requestUri = new Uri(route, UriKind.RelativeOrAbsolute);
        _testOutputHelper.WriteLine($"Sending POST to {requestUri}...");
        return await _client.PostAsync(requestUri, content, Cancellation.Token);
    }
}
