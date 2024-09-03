using HomeInventory.Web.UserManagement;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class UserManagementModuleTestContext(BaseTest test) : BaseApiModuleGivenTestContext<UserManagementModuleTestContext, UserManagementModule>(test)
{
}
