using System.Globalization;
using HomeInventory.Modules.Interfaces;
using Serilog;
using Serilog.Core;

namespace HomeInventory.Api;

public sealed class LoggingModule : BaseModule
{
    public static Logger CreateBootstrapLogger() =>
        new LoggerConfiguration()
            .Enrich.WithDemystifiedStackTraces()
            .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture, theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
            .CreateLogger();

    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((IServiceProvider provider, LoggerConfiguration loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(provider));
    }

    public override void BuildApp(IApplicationBuilder applicationBuilder, IEndpointRouteBuilder endpointRouteBuilder)
    {
        applicationBuilder.UseSerilogRequestLogging(options => options.IncludeQueryInRequestPath = true);
    }
}
