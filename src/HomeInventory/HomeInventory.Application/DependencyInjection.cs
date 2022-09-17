using System.Reflection;
using HomeInventory.Application.Authentication.Behaviors;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddStartupFilter<AddMappersFilter>();
        services.AddMappingSourceFromCurrentAssembly();
        return services;
    }

    public static IServiceCollection AddStartupFilter<T>(this IServiceCollection services)
        where T : class, IStartupFilter =>
        services.AddSingleton<IStartupFilter, T>();

    public static IServiceCollection AddMappingSourceFromCurrentAssembly(this IServiceCollection services) =>
        services.AddMappingAssemblySource(Assembly.GetCallingAssembly() ?? throw new InvalidOperationException("Calling assembly is unknown"));

    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, Assembly assembly) =>
        services.AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assembly));
}
