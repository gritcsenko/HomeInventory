using Carter;
using HomeInventory.Modules.Interfaces;
using HomeInventory.Web.Framework;
using Microsoft.AspNetCore.Builder;

namespace HomeInventory.Web;

public sealed class WebModule : BaseModuleWithCarter
{
    public override void Configure(CarterConfigurator configurator)
    {
        AddValidatorsFromCurrentAssembly(configurator);
        AddCarterModulesFromCurrentAssembly(configurator);
    }

    public override async Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken = default)
    {
        await base.BuildAppAsync(context, cancellationToken);

        context.ApplicationBuilder.UseAuthentication();
    }
}
