using HomeInventory.Contracts;

namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IAreaAPIDriver
{
    Task<AreaResponse[]> GetAllAsync();
}
