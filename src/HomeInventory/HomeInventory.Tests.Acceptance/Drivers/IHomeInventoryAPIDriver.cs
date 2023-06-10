namespace HomeInventory.Tests.Acceptance.Drivers;

internal interface IHomeInventoryApiDriver : IApiDriver
{
    IAuthenticationApiDriver Authentication { get; }

    IUserManagementApiDriver UserManagement { get; }

    IAreaApiDriver Area { get; }

    void SetToday(DateOnly today);
}
