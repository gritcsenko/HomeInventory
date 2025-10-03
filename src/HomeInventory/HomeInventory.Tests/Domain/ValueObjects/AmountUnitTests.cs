using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Domain.ValueObjects;

[UnitTest]
public class AmountUnitTests : BaseTest
{
    private static readonly EnumerationItemsCollection<AmountUnit> _items = EnumerationItemsCollection.CreateFor<AmountUnit>();

    [Fact]
    public void Items_Should_NotBeEmpty() => _items.Should().NotBeEmpty();

    [Fact]
    public void CreateShouldPassTheCallerMemberNameAndType()
    {
        var type = MeasurementType.Create();
        var sut = AmountUnit.Create(type);

        sut.Name.Should().Be(nameof(CreateShouldPassTheCallerMemberNameAndType));
        sut.Measurement.Should().BeSameAs(type);
    }

    [Fact]
    public void CreatedShouldContainSuppliedId()
    {
        var expected = Ulid.NewUlid();
        var supplier = Substitute.For<IIdSupplier<Ulid>>();
        supplier.SupplyNew().Returns(expected);
        var type = MeasurementType.Create();
        var sut = AmountUnit.Create(type, supplier);

        sut.Value.Should().Be(expected);
    }

    [Fact]
    public void CreatedScaledShouldContainSuppliedId()
    {
        var scale = Fixture.Create<decimal>();
        var expected = Ulid.NewUlid();
        var supplier = Substitute.For<IIdSupplier<Ulid>>();
        supplier.SupplyNew().Returns(expected);
        var type = MeasurementType.Create();
        var baseUnit = AmountUnit.Create(type);
        var sut = AmountUnit.Create(baseUnit, x => x * scale, supplier);

        sut.Value.Should().Be(expected);
    }

    [Fact]
    public void FieldsShouldHaveMatchedName()
    {
        var fields = typeof(AmountUnit).GetFieldsOfType<AmountUnit>().ToArray();

        fields.Should().NotBeEmpty()
            .And.AllSatisfy(static t => t.Value!.Name.Should().Be(t.Field.Name));
    }

    [Fact]
    public void CanBeUsedAsDictionaryKey()
    {
        var dictionary = _items.ToDictionary(static x => x, static x => x.Name);
        var values = typeof(AmountUnit).GetFieldValuesOfType<AmountUnit>().ToArray();

        dictionary.Should().ContainKeys(values);
    }
}
