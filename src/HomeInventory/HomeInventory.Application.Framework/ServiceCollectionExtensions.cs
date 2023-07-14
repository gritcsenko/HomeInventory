using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, params Assembly[] assemblies) =>
        services.AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assemblies));
}
