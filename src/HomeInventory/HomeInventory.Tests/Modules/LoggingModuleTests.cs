using HomeInventory.Api;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HomeInventory.Tests.Modules;

public sealed class LoggingModuleTests() : BaseModuleTest<LoggingModuleTestsGivenContext, LoggingModule>(t => new(t))
{
    [Fact]
    public void ShouldRegisterServices()
    {
        Given
            .Services(out var services)
            .Configuration(out var configuration)
            .Sut(out var sut);

        var then = When
            .Invoked(sut, services, configuration, (sut, services, configuration) => sut.AddServices(services, configuration));

        then
            .Ensure(services, services => {
                services.Should().ContainSingleSingleton<Serilog.ILogger>()
                    .And.ContainSingleSingleton<ILoggerFactory>()
                    .And.ContainSingleSingleton<IDiagnosticContext>();
            });
    }
}
