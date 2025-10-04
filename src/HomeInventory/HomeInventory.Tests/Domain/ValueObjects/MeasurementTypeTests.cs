using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Domain.ValueObjects;

[UnitTest]
public class MeasurementTypeTests : BaseTest
{
    private static readonly EnumerationItemsCollection<MeasurementType> _items = EnumerationItemsCollection.CreateFor<MeasurementType>();

    [Fact]
    public void Items_Should_NotBeEmpty() => _items.Should().NotBeEmpty();

    [Fact]
    public void CreateShouldPassTheCallerMemberName()
    {
        var sut = MeasurementType.Create();

        sut.Name.Should().Be(nameof(CreateShouldPassTheCallerMemberName));
    }

    [Fact]
    public void CreatedShouldContainSuppliedId()
    {
        var expected = Ulid.NewUlid();
        var supplier = Substitute.For<IIdSupplier<Ulid>>();
        supplier.Supply().Returns(expected);
        var sut = MeasurementType.Create(supplier);

        sut.Value.Should().Be(expected);
    }

    [Fact]
    public void FieldsShoulHaveMatchedName()
    {
        var fields = typeof(MeasurementType).GetFieldsOfType<MeasurementType>().ToArray();

        fields.Should().NotBeEmpty()
            .And.AllSatisfy(static t => t.Value!.Name.Should().Be(t.Field.Name));
    }

    [Fact]
    public void CanBeUsedAsDictionaryKey()
    {
        var dictionary = _items.ToDictionary(static x => x, static x => x.Name);
        var values = typeof(MeasurementType).GetFieldValuesOfType<MeasurementType>().ToArray();

        dictionary.Should().ContainKeys(values);
    }
}
