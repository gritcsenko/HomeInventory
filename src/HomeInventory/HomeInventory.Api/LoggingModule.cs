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

    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services.AddSerilog((IServiceProvider provider, LoggerConfiguration loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(provider));
    }

    public override async Task BuildAppAsync(ModuleBuildContext context)
    {
        await base.BuildAppAsync(context);

        context.ApplicationBuilder.UseSerilogRequestLogging(options => options.IncludeQueryInRequestPath = true);
    }
}
