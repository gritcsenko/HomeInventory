using HomeInventory.Application.Framework;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Web.Mapping;

public sealed class WebMappingModule : BaseModule
{
    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services
            .AddMappingAssemblySource(GetType().Assembly)
            .AddAutoMapper((sp, configExpression) =>
            {
                configExpression.AddMaps(sp.GetServices<IMappingAssemblySource>().SelectMany(s => s.GetAssemblies()));
                configExpression.ConstructServicesUsing(sp.GetRequiredService);
            }, Type.EmptyTypes);
    }
}
