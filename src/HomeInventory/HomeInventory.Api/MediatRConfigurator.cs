using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

internal static class MediatRConfigurator
{
    public static IServiceCollection AddMessageHub(this IServiceCollection services, params Assembly[] serviceAssemblies)
    {
        services.AddMessageHubCore();
        foreach (var serviceAssembly in serviceAssemblies)
        {
            services.AddMessageHubServicesFrom(serviceAssembly);
        }
        return services;
    }
}
