using Carter;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Web.Framework;

public sealed class WebCarterSupportModule : BaseModule
{
    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services.AddCarter(
            assemblyCatalog: null,
            configurator =>
            {
                foreach (var module in context.Modules.OfType<IModuleWithCarter>())
                {
                    module.Configure(configurator);
                }
            });
    }

    public override async Task BuildAppAsync(ModuleBuildContext context)
    {
        await base.BuildAppAsync(context);

        context.EndpointRouteBuilder.MapCarter();
    }
}
