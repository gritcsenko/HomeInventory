using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.UserManagement;

public sealed class WebUserManagementMappingModule : BaseModule
{
    public WebUserManagementMappingModule() => DependsOn<ApplicationMappingModule>();

    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services.AddMappingAssemblySource(GetType().Assembly);
    }
}
