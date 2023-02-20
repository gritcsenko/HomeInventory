using HomeInventory.Contracts;
using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class AreaAPIDriver : ApiDriver, IAreaAPIDriver
{
    public AreaAPIDriver(TestServer server)
        : base(server, "/api/areas")
    {
    }

    public async Task<AreaResponse[]> GetAllAsync()
    {
        return await GetAsync<AreaResponse[]>("");
    }
}
