using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class AreaAPIDriver : ApiDriver, IAreaAPIDriver
{
    public AreaAPIDriver(TestServer server)
        : base(server, "/api/areas")
    {
    }

    public IAsyncEnumerable<AreaResponse> GetAllAsync() =>
        GetAsync<AreaResponse[]>()
            .ToAsyncEnumerable()
            .SelectMany(x => x.ToAsyncEnumerable());
}
