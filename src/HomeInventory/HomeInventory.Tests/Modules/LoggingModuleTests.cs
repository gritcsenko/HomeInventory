using HomeInventory.Api;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HomeInventory.Tests.Modules;

public sealed class LoggingModuleTests() : BaseModuleTest<LoggingModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should().ContainSingleSingleton<Serilog.ILogger>()
            .And.ContainSingleSingleton<ILoggerFactory>()
            .And.ContainSingleSingleton<IDiagnosticContext>();
}
