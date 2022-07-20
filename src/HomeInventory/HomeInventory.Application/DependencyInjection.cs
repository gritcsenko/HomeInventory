using HomeInventory.Application.Authentication.Behaviors;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HomeInventory.Tests")]

namespace HomeInventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        TypeAdapterConfig.GlobalSettings.Scan(currentAssembly);
        services.AddMediatR(currentAssembly);
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }

    public static OptionsBuilder<TOptions> FromConfiguration<TOptions>(this OptionsBuilder<TOptions> builder)
        where TOptions : class
        => builder.FromConfiguration(typeof(TOptions).Name);

    public static OptionsBuilder<TOptions> FromConfiguration<TOptions>(this OptionsBuilder<TOptions> builder, string name)
        where TOptions : class
        => builder.Configure<IConfiguration>((options, configuration) => configuration.Bind(name, options));
}
