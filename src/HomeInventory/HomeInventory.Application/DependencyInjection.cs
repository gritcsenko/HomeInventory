using System.Reflection;
using HomeInventory.Application.Authentication.Behaviors;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

    public static TOptions AddOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, new() =>
        services.AddOptions(configuration, new TOptions());

    public static TOptions AddOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, TOptions options)
        where TOptions : class
    {
        configuration.Bind(typeof(TOptions).Name, options);
        services.AddSingleton(Options.Create(options));
        return options;
    }

    public static OptionsBuilder<TOptions> FromConfiguration<TOptions>(this OptionsBuilder<TOptions> builder)
        where TOptions : class =>
        builder.FromConfiguration(typeof(TOptions).Name);

    public static OptionsBuilder<TOptions> FromConfiguration<TOptions>(this OptionsBuilder<TOptions> builder, string name)
        where TOptions : class =>
        builder.Configure<IConfiguration>((options, configuration) => configuration.Bind(name, options));

    public static IServiceCollection AddStartupFilter<T>(this IServiceCollection services)
        where T : class, IStartupFilter =>
        services.AddSingleton<IStartupFilter, T>();

    public static IServiceCollection AddMappingSourceFromCurrentAssembly(this IServiceCollection services) =>
        services.AddMappingAssemblySource(Assembly.GetCallingAssembly() ?? throw new InvalidOperationException("Calling assembly is unknown"));

    public static IServiceCollection AddMappingAssemblySource(this IServiceCollection services, Assembly assembly) =>
        services.AddSingleton<IMappingAssemblySource>(sp => new MappingAssemblySource(assembly));
}
