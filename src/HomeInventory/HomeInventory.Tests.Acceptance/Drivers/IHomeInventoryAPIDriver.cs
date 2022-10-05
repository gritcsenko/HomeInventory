namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IHomeInventoryAPIDriver : IApiDriver
{
    IAuthenticationAPIDriver Authentication { get; }

    void SetToday(DateOnly today);
}


