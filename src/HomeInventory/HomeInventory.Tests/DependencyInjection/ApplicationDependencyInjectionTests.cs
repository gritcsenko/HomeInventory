using HomeInventory.Application;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class ApplicationDependencyInjectionTests : BaseDependencyInjectionTest
{
    [Fact]
    public void ShouldRegister()
    {
        Services.AddApplication();
        var provider = CreateProvider();

        Services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
    }
}
