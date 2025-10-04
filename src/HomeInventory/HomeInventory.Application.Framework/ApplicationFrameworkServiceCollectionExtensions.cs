using System.Reflection;
using HomeInventory.Application.Framework;
using HomeInventory.Application.Framework.Mapping;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130

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
