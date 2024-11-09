using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.Framework;

public sealed class ApplicationMappingModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services
            .AddAutoMapper((sp, configExpression) =>
            {
                configExpression.AddMaps(sp.GetServices<IMappingAssemblySource>().SelectMany(s => s.GetAssemblies()));
                configExpression.ConstructServicesUsing(sp.GetRequiredService);
            }, Type.EmptyTypes);
    }
}
