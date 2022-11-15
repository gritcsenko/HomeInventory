using System.Reflection;
using FluentValidation;
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
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddSingleton<IStartupFilter, AddMappersFilter>();
        services.AddMappingAssemblySource(AssemblyReference.Assembly);
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);
        return services;
    }

    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, Assembly assembly) =>
        services.AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assembly));
}
