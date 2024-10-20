using Carter;
using HomeInventory.Web.Framework;

namespace HomeInventory.Contracts.Validations;

public sealed class ContractsValidationsModule : BaseModuleWithCarter
{
    public override void Configure(CarterConfigurator configurator)
    {
        AddValidatorsFromCurrentAssembly(configurator);
    }
}
