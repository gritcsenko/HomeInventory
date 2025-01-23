using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using HomeInventory.Modules.Interfaces;
using Serilog;
using Serilog.Core;

namespace HomeInventory.Api;

[SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Module")]
public sealed class LoggingModule : BaseModule
{
    public static Logger CreateBootstrapLogger() =>
        new LoggerConfiguration()
            .Enrich.WithDemystifiedStackTraces()
            .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture, theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
            .CreateLogger();

    public override async Task AddServicesAsync(IModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services.AddSerilog((provider, loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(provider));
    }

    public override async Task BuildAppAsync(IModuleBuildContext context)
    {
        await base.BuildAppAsync(context);

        context.ApplicationBuilder.UseSerilogRequestLogging(static options => options.IncludeQueryInRequestPath = true);
    }
}
