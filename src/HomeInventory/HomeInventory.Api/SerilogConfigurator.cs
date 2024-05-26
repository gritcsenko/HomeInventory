using System.Globalization;
using Serilog;
using Serilog.Core;

namespace Microsoft.Extensions.DependencyInjection;

internal static class SerilogConfigurator
{
    public static Logger CreateBootstrapLogger() =>
        new LoggerConfiguration()
            .Enrich.WithDemystifiedStackTraces()
            .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture, theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
            .CreateLogger();

    public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSerilog((IServiceProvider provider, LoggerConfiguration loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(provider));
}
