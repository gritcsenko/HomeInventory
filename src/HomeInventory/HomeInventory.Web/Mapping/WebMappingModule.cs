using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Web.Mapping;

public sealed class WebMappingModule : BaseModule
{
    public WebMappingModule()
    {
        DependsOn<ApplicationMappingModule>();
    }

    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services
            .AddMappingAssemblySource(GetType().Assembly);
    }
}
