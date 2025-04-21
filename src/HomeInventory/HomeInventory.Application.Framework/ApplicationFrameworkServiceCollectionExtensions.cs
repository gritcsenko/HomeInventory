using System.Reflection;
using HomeInventory.Application.Framework.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application.Framework;

public static class ApplicationFrameworkServiceCollectionExtensions
{
    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, params Assembly[] assemblies) =>
        services
            .AddMappingTypeConverter()
            .AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assemblies));

    public static IServiceCollection AddMappingTypeConverter(this IServiceCollection services) =>
        services
            .AddSingleton(typeof(TypeConverterAdapter<,,>));
}
