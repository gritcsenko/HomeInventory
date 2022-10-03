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
        services.AddSingleton<IStartupFilter, AddMappersFilter>();
        services.AddMappingAssemblySource(AssemblyReference.Assembly);
        return services;
    }

    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, Assembly assembly) =>
        services.AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assembly));
}
