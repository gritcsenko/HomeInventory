using Serilog;

namespace HomeInventory.Api;

internal static class SerilogConfigurator
{
    public static void Configure(HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
}
