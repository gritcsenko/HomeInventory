using HomeInventory.Web.Modules;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class AuthenticationModuleTestContext(BaseTest test) : BaseApiModuleGivenTestContext<AuthenticationModuleTestContext, AuthenticationModule>(test)
{
}
