using HomeInventory.Application.Framework;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure;

public sealed class InfrastructureMappingModule : BaseModule
{
    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services
            .AddSingleton<AmountObjectConverter>()
            .AddMappingAssemblySource(GetType().Assembly);
    }
}
