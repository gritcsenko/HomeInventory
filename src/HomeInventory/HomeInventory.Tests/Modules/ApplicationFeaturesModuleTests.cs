using Microsoft.FeatureManagement;

namespace HomeInventory.Tests.Modules;

public class ApplicationFeaturesModuleTests() : BaseModuleTest<ApplicationFeaturesModule>(() => new())
{
    protected override void EnsureRegistered(IServiceCollection services)
    {
        services.Should().ContainSingleSingleton<IFeatureDefinitionProvider>();
    }
}
