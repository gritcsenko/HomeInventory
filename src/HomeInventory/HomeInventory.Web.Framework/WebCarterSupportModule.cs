using Carter;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Web.Framework;

public sealed class WebCarterSupportModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

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

    public override async Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken = default)
    {
        await base.BuildAppAsync(context, cancellationToken);

        context.EndpointRouteBuilder.MapCarter();
    }
}
