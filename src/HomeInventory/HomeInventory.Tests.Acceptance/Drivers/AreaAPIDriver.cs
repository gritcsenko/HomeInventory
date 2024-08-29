using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class AreaApiDriver(TestServer server) : ApiDriver(server, "/api/areas"), IAreaApiDriver
{
    public IAsyncEnumerable<AreaResponse> GetAllAsync() =>
        CreateGetRequest("")
            .SendAsync<AreaResponse[]>()
            .ToAsyncEnumerable()
            .SelectMany(x => x.ToAsyncEnumerable());
}
