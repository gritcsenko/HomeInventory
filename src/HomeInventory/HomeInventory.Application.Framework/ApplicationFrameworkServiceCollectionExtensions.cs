using System.Reflection;
using HomeInventory.Application.Framework;
using HomeInventory.Application.Framework.Mapping;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationFrameworkServiceCollectionExtensions
{
    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, params Assembly[] assemblies) =>
        services
            .AddMappingTypeConverter()
            .AddSingleton<IMappingAssemblySource>(_ => new MappingAssemblySource(assemblies));

    private static IServiceCollection AddMappingTypeConverter(this IServiceCollection services) =>
        services
            .AddSingleton(typeof(TypeConverterAdapter<,,>));
}
