using Microsoft.FeatureManagement;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddFeatureManagement();
        return services;
    }
}
