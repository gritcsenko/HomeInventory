using System.Diagnostics.CodeAnalysis;
using HomeInventory.Web;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class WebHealthCheckModuleTests() : BaseModuleTest<WebHealthCheckModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should()
            .Contain(d => d.ServiceType == typeof(HealthCheckService));
}

