using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.Framework;

public static class ApplicationFrameworkServiceCollectionExtensions
{
    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, params Assembly[] assemblies)
    {
        ////services.TryAddSingleton(typeof(TypeConverterAdapter<,,>));
        services.AddSingleton<IMappingAssemblySource>(_ => new MappingAssemblySource(assemblies));
        return services;
    }
}
