using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Web.UserManagement;

public sealed class WebUerManagementMappingModule : BaseModule
{
    public WebUerManagementMappingModule() => DependsOn<ApplicationMappingModule>();

    public override async Task AddServicesAsync(IModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services.AddMappingAssemblySource(GetType().Assembly);
    }
}
