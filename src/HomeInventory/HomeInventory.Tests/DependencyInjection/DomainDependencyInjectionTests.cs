using FluentAssertions.Execution;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class DomainDependencyInjectionTests : BaseDependencyInjectionTest
{
    [Fact]
    public void ShouldRegister()
    {
        Services.AddDomain();
        var provider = CreateProvider();

        VerifyTimeServices(provider);
    }

    private void VerifyTimeServices(IServiceProvider provider)
    {
        using var scope = new AssertionScope();
        Services.Should().ContainSingleTransient<TimeProvider>(provider);
    }
}
