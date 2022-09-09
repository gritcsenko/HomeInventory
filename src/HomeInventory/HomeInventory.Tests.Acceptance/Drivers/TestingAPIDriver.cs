using Microsoft.AspNetCore.TestHost;

namespace HomeInventory.Tests.Acceptance.Drivers;

internal class TestingAPIDriver : ITestingAPIDriver
{
    private const string ControllerPath = "/api/Testing";
    private TestServer _server;

    public TestingAPIDriver(TestServer server)
    {
        _server = server;
    }

    public async Task ClearDatabaseAsync()
    {
        var result = await _server.CreateRequest(ControllerPath + "/clearDatabase")
            .PostAsync();

        result.EnsureSuccessStatusCode();
    }
}
