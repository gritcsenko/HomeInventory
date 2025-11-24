using System.Diagnostics.CodeAnalysis;
using HomeInventory.Web;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class WebModuleTests() : BaseModuleTest<WebModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should()
            .ContainSingleSingleton<ContractsMapper>();
}

