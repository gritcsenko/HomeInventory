using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Modules.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Domain;

public sealed class DomainModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(IdSuppliers.Ulid);
        services.AddSingleton<IScopeFactory, ScopeFactory>();
        services.AddSingleton<IScopeContainer, ScopeContainer>();
        services.AddSingleton<IScopeAccessor, ScopeAccessor>();
        services.AddSingleton<IAmountFactory, AmountFactory>();
        services.AddTransient<TimeProvider>(_ => new FixedTimeProvider(TimeProvider.System));
    }
}
