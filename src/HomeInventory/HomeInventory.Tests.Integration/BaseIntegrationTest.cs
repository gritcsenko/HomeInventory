using System.Net.Http.Json;
using HomeInventory.Domain.Primitives;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Integration;

[IntegrationTest]
public abstract class BaseIntegrationTest : BaseTest
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly WebApplicationFactory<Program> _appFactory = new();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Used in AddDisposable")]
    private readonly HttpClient _client;

    protected BaseIntegrationTest()
    {
        AddDisposable(_appFactory);
        _client = _appFactory.CreateClient();
        AddDisposable(_client);
    }

    protected IReadOnlyCollection<RouteEndpoint> Endpoints => _appFactory
        .Services
        .GetServices<EndpointDataSource>()
        .SelectMany(s => s.Endpoints)
        .OfType<RouteEndpoint>()
        .ToReadOnly();

    protected async Task<HttpResponseMessage> PostAsync(string route, JsonContent content) =>
        await _client.PostAsync(new Uri(route, UriKind.RelativeOrAbsolute), content, Cancellation.Token);
}
