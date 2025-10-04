namespace HomeInventory.Tests.Acceptance.Drivers;

internal interface IHomeInventoryApiDriver : IApiDriver
{
    IUserManagementApiDriver UserManagement { get; }

    void SetToday(DateOnly today);
}
