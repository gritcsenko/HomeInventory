using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.Framework;

public sealed class ApplicationMappingModule : BaseModule
{
    public override async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default)
    {
        await base.AddServicesAsync(context, cancellationToken);

        context.Services
            .AddSingleton(typeof(TypeConverterAdapter<,,>))
            .AddAutoMapper(static (sp, configExpression) =>
            {
                configExpression.AddMaps(sp.GetServices<IMappingAssemblySource>().SelectMany(static s => s.GetAssemblies()));
                configExpression.ConstructServicesUsing(sp.GetRequiredService);
            }, Type.EmptyTypes);
    }
}
