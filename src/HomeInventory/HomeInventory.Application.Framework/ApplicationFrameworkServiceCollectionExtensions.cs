using System.Reflection;
using HomeInventory.Application.Framework.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeInventory.Application.Framework;

public static class ApplicationFrameworkServiceCollectionExtensions
{
    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.TryAddSingleton(typeof(TypeConverterAdapter<,,>));
        services.AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assemblies));
        return services;
    }
}
