using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<IAmountFactory, AmountFactory>();
        services.AddSingleton<SystemDateTimeService>();
        services.AddScoped<IDateTimeService>(sp => new FixedDateTimeService(sp.GetRequiredService<SystemDateTimeService>()));
        return services;
    }
}
