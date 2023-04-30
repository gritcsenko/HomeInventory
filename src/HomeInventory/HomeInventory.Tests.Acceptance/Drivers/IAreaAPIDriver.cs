using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IAreaAPIDriver
{
    IAsyncEnumerable<AreaResponse> GetAllAsync();
}
