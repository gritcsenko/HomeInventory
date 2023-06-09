using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMappingAssemblySource(AssemblyReference.Assembly);
        return services;
    }

    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, Assembly assembly) =>
        services.AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assembly));
}
