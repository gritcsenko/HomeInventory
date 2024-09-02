using System.Reflection;
using HomeInventory.Application;
using HomeInventory.Application.Framework.Mapping;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationFrameworkServiceCollectionExtensions
{
    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.TryAddSingleton(typeof(TypeConverterAdapter<,,>));
        services.AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assemblies));
        return services;
    }
}
