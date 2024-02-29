using HomeInventory.Domain;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives;
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
    public void ShouldProjectStorageAreaModelToStorageArea()
    {
        Fixture.CustomizeUlidId<StorageAreaId>();
        var sut = CreateSut<ModelMappings>();
        var instance = Fixture.Create<StorageAreaModel>();
        var source = new[] { instance }.AsQueryable();

        var target = sut.ProjectTo<StorageArea>(source, Cancellation.Token).ToArray();

        target.Should().ContainSingle();
    }

    public static TheoryData<object, Type> MapData()
    {
        var fixture = new Fixture();
        fixture.CustomizeUlid();
        fixture.CustomizeUlidId<ProductId>();
        fixture.CustomizeUlidId<MaterialId>();
        fixture.CustomizeUlidId<StorageAreaId>();
        fixture.CustomizeFromFactory<string, StorageAreaName>(x => new StorageAreaName(x));

        var items = EnumerationItemsCollection.CreateFor<AmountUnit>();
        fixture.CustomizeFromFactory<int, AmountUnit>(i => items.ElementAt(i % items.Count));
        fixture.CustomizeFromFactory<(decimal value, AmountUnit unit), Amount>(x => new Amount(x.value, x.unit));

        fixture.Customize<ProductAmountModel>(builder =>
            builder.With(m => m.UnitName, (AmountUnit unit) => unit.Name));

        var data = new TheoryData<object, Type>();

        Add<ProductId, Ulid>(fixture, data);
        Add<MaterialId, Ulid>(fixture, data);
        Add<StorageAreaId, Ulid>(fixture, data);

        Add<StorageAreaName, string>(fixture, data);

        Add<Amount, ProductAmountModel>(fixture, data);
        Add<Product, ProductModel>(fixture, data);
        Add<StorageArea, StorageAreaModel>(fixture, data);

        return data;

        static void Add<T1, T2>(IFixture fixture, TheoryData<object, Type> data)
            where T1 : notnull
            where T2 : notnull
        {
            data.Add(fixture.Create<T1>(), typeof(T2));
            data.Add(fixture.Create<T2>(), typeof(T1));
        }
    }
}
