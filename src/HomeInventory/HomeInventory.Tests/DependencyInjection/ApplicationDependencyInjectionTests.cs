using HomeInventory.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class ApplicationDependencyInjectionTests : BaseDependencyInjectionTest
{
    [Fact]
    public void ShouldRegister()
    {
        Services
            .AddSubstitute<IConfiguration>()
            .AddApplication();
        var provider = CreateProvider();

        Services.Should().ContainSingleSingleton<IFeatureDefinitionProvider>(provider);
    }
}
