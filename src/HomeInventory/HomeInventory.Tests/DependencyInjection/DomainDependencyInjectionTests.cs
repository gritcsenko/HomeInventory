using HomeInventory.Domain;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class DomainDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new TestingServiceCollection();

    [Fact]
    public void ShouldRegister()
    {
        _services.AddDomain();

        _services.Should().ContainSingleTransient<IUserIdFactory>();
        _services.Should().ContainSingleTransient<IMaterialIdFactory>();
        _services.Should().ContainSingleTransient<IProductIdFactory>();
    }
}
