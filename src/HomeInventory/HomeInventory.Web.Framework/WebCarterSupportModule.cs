using Carter;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Web.Framework;

public sealed class WebCarterSupportModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context)
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

    public override async Task BuildAppAsync(IModuleBuildContext context)
    {
        await base.BuildAppAsync(context);

        context.EndpointRouteBuilder.MapCarter();
    }
}
