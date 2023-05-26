namespace HomeInventory.Tests.Acceptance.Drivers;

internal interface IHomeInventoryAPIDriver : IApiDriver
{
    IUserManagementAPIDriver UserManagement { get; }

    void SetToday(DateOnly today);
}
