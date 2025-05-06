using Carter;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Web.Framework;

public interface IModuleWithCarter : IModule
{
    void Configure(CarterConfigurator configurator);
}
