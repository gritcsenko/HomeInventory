using HomeInventory.Web.Modules;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class UserManagementModuleTestContext(BaseTest test) : BaseApiModuleGivenTestContext<UserManagementModuleTestContext, UserManagementModule>(test)
{
}
