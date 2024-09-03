using Carter;
using HomeInventory.Web.Framework;

namespace HomeInventory.Web.UserManagement;

public sealed class WebUserManagementModule : BaseModuleWithCarter
{
    public override void Configure(CarterConfigurator configurator)
    {
        AddCarterModulesFromCurrentAssembly(configurator);
    }
}
