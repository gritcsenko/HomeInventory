using System.Reflection;
using HomeInventory.Application;
using HomeInventory.Application.Framework.Mapping;
using HomeInventory.Domain.Primitives.Messages;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationFrameworkServiceCollectionExtensions
{
    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, params Assembly[] assemblies) =>
        services
            .AddMappingTypeConverter()
            .AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assemblies));

    public static IServiceCollection AddMappingTypeConverter(this IServiceCollection services) =>
        services
            .AddSingleton(typeof(TypeConverterAdapter<,,>));

    public static IServiceCollection AddMessageHubCore(this IServiceCollection services)
    {
        services.AddSingleton<IMessageObservableProvider, MessageObservableProvider>();
        services.AddSingleton<IMessageHub, MessageHub>();
        services.AddSingleton(typeof(RequestHandlerAdapter<,>));
        services.AddSingleton(typeof(MessageHandlerAdapter<>));
        return services;
    }

    public static IServiceCollection AddMessageHubServicesFrom(this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.DefinedTypes.Where(t => t.CanBeService()))
        {
            services.TryAddEnumerable(type.GetServicesOfType(typeof(IMessageHandler<>)));
            services.TryAddEnumerable(type.GetServicesOfType(typeof(IMessagePipelineBehavior<>)));
            services.TryAddEnumerable(type.GetServicesOfType(typeof(IRequestHandler<,>)));
            services.TryAddEnumerable(type.GetServicesOfType(typeof(IRequestPipelineBehavior<,>)));
        }

        return services;
    }

    private static IEnumerable<ServiceDescriptor> GetServicesOfType(this Type type, Type serviceTemplateType)
    {
        foreach (var iface in type.GetInterfaces().Where(i => i.IsGenericType).Where(i => i.GetGenericTypeDefinition() == serviceTemplateType))
        {
            yield return new ServiceDescriptor(iface, type, ServiceLifetime.Singleton);
        }
    }

    private static bool CanBeService(this Type type) => !type.IsOpenGeneric() && type.CanBeInstantiated();

    private static bool IsOpenGeneric(this Type type) => type.IsGenericTypeDefinition || type.ContainsGenericParameters;

    private static bool CanBeInstantiated(this Type type) => !(type.IsAbstract || type.IsInterface);
}
