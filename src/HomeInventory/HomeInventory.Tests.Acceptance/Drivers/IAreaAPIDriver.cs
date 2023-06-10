using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IAreaApiDriver
{
    IAsyncEnumerable<AreaResponse> GetAllAsync();
}
