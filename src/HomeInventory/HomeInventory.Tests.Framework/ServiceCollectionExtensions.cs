using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.Framework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSubstitute<T>(this IServiceCollection services)
        where T : class =>
        services.AddSubstitute<T>(out var _);

    public static IServiceCollection AddSubstitute<T>(this IServiceCollection services, out T substutute)
        where T : class
    {
        substutute = Substitute.For<T>();
        return services.AddSingleton(substutute);
    }

    public static IServiceCollection AddOptions<T>(this IServiceCollection services, T options)
        where T : class =>
        services.AddSingleton(Options.Create(options));
}
