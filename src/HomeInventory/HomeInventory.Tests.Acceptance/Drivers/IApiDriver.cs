using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IApiDriver : IDisposable, IAsyncDisposable
{
    TestServer Server { get; }

    IServiceProvider Services { get; }

    WebApplicationFactoryClientOptions ClientOptions { get; }

    HttpClient CreateClient();

    HttpClient CreateClient(WebApplicationFactoryClientOptions options);

    HttpClient CreateDefaultClient(params DelegatingHandler[] handlers);

    HttpClient CreateDefaultClient(Uri baseAddress, params DelegatingHandler[] handlers);
}
