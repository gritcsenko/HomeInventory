using HomeInventory.Domain;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.DependencyInjection;

[Trait("Category", "Unit")]
public class DomainDependencyInjectionTests : BaseTest
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IServiceProviderFactory<IServiceCollection> _factory = new DefaultServiceProviderFactory();

    [Fact]
    public void ShouldRegister()
    {
        _services.AddDomain();
        var provider = _factory.CreateServiceProvider(_services);

        _services.Should().ContainSingleTransient<IUserIdFactory>(provider);
        _services.Should().ContainSingleTransient<IMaterialIdFactory>(provider);
        _services.Should().ContainSingleTransient<IProductIdFactory>(provider);
    }
}
