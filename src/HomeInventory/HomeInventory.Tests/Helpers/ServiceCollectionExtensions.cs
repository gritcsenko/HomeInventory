using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeInventory.Tests.Helpers;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection ReplaceWithSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
        where TService : class =>
        services.ReplaceWith(factory, ServiceLifetime.Singleton);

    public static IServiceCollection ReplaceWithScoped<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
        where TService : class =>
        services.ReplaceWith(factory, ServiceLifetime.Scoped);

    public static IServiceCollection ReplaceWith<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory, ServiceLifetime lifetime)
        where TService : class =>
        services.Replace(new ServiceDescriptor(typeof(TService), factory, lifetime));

}
