using HomeInventory.Domain;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class ModelMappingsTests : BaseMappingsTests
{
    public ModelMappingsTests()
    {
        Services.AddDomain();
        Services.AddInfrastructure();
    }

    [Theory]
    [MemberData(nameof(MapData))]
    public void ShouldMap(object instance, Type destination)
    {
        var sut = CreateSut<ModelMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    [Fact]
    public void ShouldProjectUserModelToUser()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        var sut = CreateSut<ModelMappings>();
        var instance = Fixture.Create<UserModel>();
        var source = new[] { instance }.AsQueryable();

        var target = sut.ProjectTo<User>(source, Cancellation.Token).ToArray();

        target.Should().HaveCount(1);
    }

    [Fact]
    public void ShouldMapUserModelToUser()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        var sut = CreateSut<ModelMappings>();
        var instance = Fixture.Create<UserModel>();
        var source = new[] { instance }.AsQueryable();

        var target = sut.Map<User>(instance);

        target.Id.Id.Should().Be(instance.Id.Id);
        target.Email.Value.Should().Be(instance.Email);
        target.Password.Should().Be(instance.Password);
    }

    public static TheoryData<object, Type> MapData()
    {
        var fixture = new Fixture();
        fixture.CustomizeGuidId(guid => new UserId(guid));
        fixture.CustomizeGuidId(guid => new StorageAreaId(guid));
        fixture.CustomizeGuidId(guid => new ProductId(guid));
        fixture.CustomizeEmail();
        fixture.CustomizeFromFactory<int, AmountUnit>(i => AmountUnit.Items.ElementAt(i % AmountUnit.Items.Count));
        fixture.CustomizeFromFactory<(decimal value, AmountUnit unit), Amount>(x => new Amount(x.value, x.unit));
        fixture.CustomizeFromFactory<string, StorageAreaName>(x => new StorageAreaName(x));

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
