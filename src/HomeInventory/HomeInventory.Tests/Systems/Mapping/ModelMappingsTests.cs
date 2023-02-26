using HomeInventory.Domain;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Tests.Systems.Mapping;

[Trait("Category", "Unit")]
public class ModelMappingsTests : BaseMappingsTests
{
    public ModelMappingsTests()
    {
        Services.AddDomain();
        Services.AddInfrastructure();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldMap(object instance, Type destination)
    {
        var sut = CreateSut<ModelMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<object, Type> Data()
    {
        var fixture = new Fixture();
        fixture.CustomizeGuidId(guid => new UserId(guid));
        fixture.CustomizeGuidId(guid => new StorageAreaId(guid));
        fixture.CustomizeGuidId(guid => new ProductId(guid));
        fixture.CustomizeEmail();
        fixture.Customize(new FromFactoryCustomization<int, AmountUnit>(i => AmountUnit.Items.ElementAt(i % AmountUnit.Items.Count)));
        fixture.Customize(new FromFactoryCustomization<(decimal value, AmountUnit unit), Amount>(x => new Amount(x.value, x.unit)));
        fixture.Customize(new FromFactoryCustomization<string, StorageAreaName>(x => new StorageAreaName(x)));

        fixture.Customize<ProductAmountModel>(builder =>
            builder.With(m => m.UnitName, (AmountUnit unit) => unit.Name));

        return new()
        {
            { fixture.Create<UserId>(), typeof(Guid) },
            { fixture.Create<Guid>(), typeof(UserId) },

            { fixture.Create<StorageAreaId>(), typeof(Guid) },
            { fixture.Create<Guid>(), typeof(StorageAreaId) },

            { fixture.Create<ProductId>(), typeof(Guid) },
            { fixture.Create<Guid>(), typeof(ProductId) },

            { fixture.Create<Email>(), typeof(string) },
            { fixture.Create<string>(), typeof(Email) },

            { fixture.Create<StorageAreaName>(), typeof(string) },
            { fixture.Create<string>(), typeof(StorageAreaName) },

            { fixture.Create<User>(), typeof(UserModel) },
            { fixture.Create<UserModel>(), typeof(User) },

            { fixture.Create<Amount>(), typeof(ProductAmountModel) },
            { fixture.Create<ProductAmountModel>(), typeof(Amount) },

            { fixture.Create<Product>(), typeof(ProductModel) },
            { fixture.Create<ProductModel>(), typeof(Product) },

            { fixture.Create<StorageArea>(), typeof(StorageAreaModel) },
            { fixture.Create<StorageAreaModel>(), typeof(StorageArea) },
        };
    }
}
