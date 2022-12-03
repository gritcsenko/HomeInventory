using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
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


        VerifyIdFactory<UserId>(provider);
        VerifyIdFactory<MaterialId>(provider);
        VerifyIdFactory<ProductId>(provider);
        VerifyIdFactory<StorageAreaId>(provider);
        VerifyValueFactory<Email, string, EmailFactory>(provider);
    }

    private void VerifyValueFactory<TObject, TValue, TFactory>(IServiceProvider provider)
        where TObject : class, IValueObject<TObject>
        where TFactory : class, IValueObjectFactory<TObject, TValue>
    {
        _services.Should().ContainSingleSingleton<TFactory>(provider);
        _services.Should().ContainSingleSingleton<IValueObjectFactory<TObject, TValue>>(provider);
    }

    private void VerifyIdFactory<TId>(IServiceProvider provider)
       where TId : IIdentifierObject<TId>
    {
        _services.Should().ContainSingleSingleton<Func<Guid, TId>>(provider);
        _services.Should().ContainSingleSingleton<IIdFactory<TId>>(provider);

        _services.Should().ContainSingleSingleton<IIdFactory<TId, Guid>>(provider);
        _services.Should().ContainSingleSingleton<IValueObjectFactory<TId, Guid>>(provider);

        _services.Should().ContainSingleSingleton<IIdFactory<TId, string>>(provider);
        _services.Should().ContainSingleSingleton<IValueObjectFactory<TId, string>>(provider);
    }
}
