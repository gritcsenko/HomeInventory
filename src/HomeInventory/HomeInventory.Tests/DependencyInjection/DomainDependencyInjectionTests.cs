using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class DomainDependencyInjectionTests : BaseDependencyInjectionTest
{
    [Fact]
    public void ShouldRegister()
    {
        Services.AddDomain();
        var provider = CreateProvider();

        VerifyIdFactory<UserId>(provider);
        VerifyIdFactory<MaterialId>(provider);
        VerifyIdFactory<ProductId>(provider);

        VerifyValueFactory<Email, string, EmailFactory>(provider);

        VerifyTimeServices(provider);
    }

    private void VerifyTimeServices(IServiceProvider provider)
    {
        Services.Should().ContainSingleSingleton<SystemDateTimeService>(provider);
        Services.Should().ContainSingleScoped<IDateTimeService>(provider);
    }

    private void VerifyIdFactory<TId>(IServiceProvider provider)
       where TId : IIdentifierObject<TId>
    {
        Services.Should().ContainSingleSingleton<Func<Guid, TId>>(provider);
        Services.Should().ContainSingleSingleton<IIdFactory<TId>>(provider);

        VerifyValueFactory<TId, Guid, IIdFactory<TId, Guid>>(provider);
        VerifyValueFactory<TId, string, IIdFactory<TId, string>>(provider);
    }

    private void VerifyValueFactory<TObject, TValue, TFactory>(IServiceProvider provider)
        where TObject : IValueObject<TObject>
        where TFactory : class, IValueObjectFactory<TObject, TValue>
    {
        Services.Should().ContainSingleSingleton<TFactory>(provider);
        Services.Should().ContainSingleSingleton<IValueObjectFactory<TObject, TValue>>(provider);
    }
}
