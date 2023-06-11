using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class AreaApiDriver : ApiDriver, IAreaApiDriver
{
    public AreaApiDriver(TestServer server)
        : base(server, "/api/areas")
    {
    }

    public IAsyncEnumerable<AreaResponse> GetAllAsync() =>
        CreateGetRequest("")
            .SendAsync<AreaResponse[]>()
            .ToAsyncEnumerable()
            .SelectMany(x => x.ToAsyncEnumerable());
}
