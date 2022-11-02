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

        _services.Should().ContainSingleTransient<IIdFactory<UserId>>(provider);
        _services.Should().ContainSingleTransient<IIdFactory<UserId, Guid>>(provider);
        _services.Should().ContainSingleTransient<IIdFactory<MaterialId>>(provider);
        _services.Should().ContainSingleTransient<IIdFactory<MaterialId, Guid>>(provider);
        _services.Should().ContainSingleTransient<IIdFactory<ProductId>>(provider);
        _services.Should().ContainSingleTransient<IIdFactory<ProductId, Guid>>(provider);
        _services.Should().ContainSingleTransient<IIdFactory<StorageAreaId>>(provider);
        _services.Should().ContainSingleTransient<IIdFactory<StorageAreaId, Guid>>(provider);
    }
}
