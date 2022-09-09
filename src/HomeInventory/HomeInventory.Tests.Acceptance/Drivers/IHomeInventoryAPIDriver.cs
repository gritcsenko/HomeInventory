namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IHomeInventoryAPIDriver : IApiDriver
{
    IAuthenticationAPIDriver Authentication { get; }

    ITestingAPIDriver Testing { get; }

    void SetToday(DateOnly today);
}
