using FluentAssertions.Execution;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Domain.ValueObjects;

[UnitTest]
public class AmountUnitTests : BaseTest
{
    private static readonly EnumerationItemsCollection<AmountUnit> _items = EnumerationItemsCollection.CreateFor<AmountUnit>();

    [Fact]
    public void Items_Should_NotBeEmpty()
    {
        _items.Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void PropertiesShouldMatch(AmountUnit sut, string name, MeasurementType type, bool isMetric)
    {
        using var scope = new AssertionScope();
        sut.Name.Should().Be(name);
        sut.Measurement.Should().Be(type);
        sut.IsMetric.Should().Be(isMetric);
    }

    [Theory]
    [MemberData(nameof(Keys))]
    public void CanBeUsedAsDictionaryKey(AmountUnit sut)
    {
        var dictionary = _items.ToDictionary(x => x, x => x.Name);

        dictionary.Should().ContainKey(sut);
    }

    public static TheoryData<AmountUnit, string, MeasurementType, bool> Data() =>
        new()
        {
            { AmountUnit.Piece, nameof(AmountUnit.Piece), MeasurementType.Count, true },
            { AmountUnit.CubicMeter, nameof(AmountUnit.CubicMeter), MeasurementType.Volume, true },
            { AmountUnit.Kelvin, nameof(AmountUnit.Kelvin), MeasurementType.Temperature, true },
            { AmountUnit.Gallon, nameof(AmountUnit.Gallon), MeasurementType.Volume, false },
        };

    public static TheoryData<AmountUnit> Keys() => new(_items);
}
