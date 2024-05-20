using HomeInventory.Core;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<IScopeAccessor, ScopeAccessor>();
        services.AddSingleton<IAmountFactory, AmountFactory>();
        services.AddTransient<TimeProvider>(_ => new FixedTimeProvider(TimeProvider.System));
        return services;
    }
}
