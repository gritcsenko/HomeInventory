using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Infrastructure.UserManagement;

public sealed class InfrastructureUserManagementMappingModule : BaseModule
{
    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services.AddMappingAssemblySource(GetType().Assembly);
    }
}
