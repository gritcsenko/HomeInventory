using HomeInventory.Domain;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton(IdSuppliers.Ulid);
        services.AddSingleton<IScopeFactory, ScopeFactory>();
        services.AddSingleton<IScopeContainer, ScopeContainer>();
        services.AddSingleton<IScopeAccessor, ScopeAccessor>();
        services.AddSingleton<IAmountFactory, AmountFactory>();
        services.TryAddTransient<TimeProvider>(_ => new FixedTimeProvider(TimeProvider.System));
        return services;
    }
}
