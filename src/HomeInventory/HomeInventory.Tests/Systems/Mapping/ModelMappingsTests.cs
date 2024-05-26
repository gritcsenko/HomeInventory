using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Tests.Framework.Customizations;
using Visus.Cuid;

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

    public static TheoryData<object, Type> MapData()
    {
        var fixture = new Fixture();
        fixture.CustomizeId<ProductId>();

        var items = EnumerationItemsCollection.CreateFor<AmountUnit>();
        fixture.CustomizeFromFactory<AmountUnit, int>(i => items.ElementAt(i % items.Count));
        fixture.CustomizeFromFactory<Amount, (decimal value, AmountUnit unit)>(x => new Amount(x.value, x.unit));

        fixture.Customize<ProductAmountModel>(builder =>
            builder.With(m => m.UnitName, (AmountUnit unit) => unit.Name));

        var data = new TheoryData<object, Type>();

        Add<ProductId, Cuid>(fixture, data);

        Add<Amount, ProductAmountModel>(fixture, data);

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
