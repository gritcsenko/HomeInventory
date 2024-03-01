using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace HomeInventory.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddFeatureManagement();
        services.AddMappingAssemblySource(AssemblyReference.Assembly);
        return services;
    }
}
