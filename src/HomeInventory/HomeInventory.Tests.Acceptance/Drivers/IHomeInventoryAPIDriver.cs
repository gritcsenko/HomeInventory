namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IHomeInventoryAPIDriver : IApiDriver
{
    IAuthenticationAPIDriver Authentication { get; }

    IUserManagementAPIDriver UserManagement { get; }

    IAreaAPIDriver Area { get; }

    void SetToday(DateOnly today);
}
