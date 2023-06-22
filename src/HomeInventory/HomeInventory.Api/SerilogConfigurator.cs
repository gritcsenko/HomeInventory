using Serilog;

namespace HomeInventory.Api;

internal static class SerilogConfigurator
{
    public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSerilog((IServiceProvider provider, LoggerConfiguration loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(provider));
}
