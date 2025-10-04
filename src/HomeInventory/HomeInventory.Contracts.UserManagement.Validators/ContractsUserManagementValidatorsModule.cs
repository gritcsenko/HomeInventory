using Carter;
using HomeInventory.Web.Framework;

namespace HomeInventory.Contracts.UserManagement.Validators;

public sealed class ContractsUserManagementValidatorsModule : BaseModuleWithCarter
{
    public override void Configure(CarterConfigurator configurator) => AddValidatorsFromCurrentAssembly(configurator);
}
