using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeInventory.Domain;

public sealed class DomainModule : BaseModule
{
    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services
            .AddSingleton(IdSuppliers.Ulid)
            .AddSingleton<IScopeFactory, ScopeFactory>()
            .AddSingleton<IScopeContainer, ScopeContainer>()
            .AddSingleton<IScopeAccessor, ScopeAccessor>()
            .AddSingleton<IAmountFactory, AmountFactory>()
            .TryAddTransient(_ => TimeProvider.System);
    }
}
