using FluentAssertions;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Domain.ValueObjects;

[Trait("Category", "Unit")]
public class AmountUnitTests : BaseTest
{
    [Fact]
    public void Items_Should_NotBeEmpty()
    {
        AmountUnit.Items.Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void PropertiesShouldMatch(AmountUnit sut, string name, MeasurementType type, decimal factor)
    {
        sut.Name.Should().Be(name);
        sut.Measurement.Should().Be(type);
        sut.CIUnitFactor.Should().Be(factor);
    }

    [Theory]
    [MemberData(nameof(Keys))]
    public void CanBeUsedAsDictionaryKey(AmountUnit sut)
    {
        var dictionary = AmountUnit.Items.ToDictionary(x => x, x => x.Name);

        dictionary.Should().ContainKey(sut);
    }

    public static TheoryData<AmountUnit, string, MeasurementType, decimal> Data()
    {
        return new()
        {
            { AmountUnit.Piece, nameof(AmountUnit.Piece), MeasurementType.Count, 1m },
            { AmountUnit.CubicMeter, nameof(AmountUnit.CubicMeter), MeasurementType.Volume, 1m },
            { AmountUnit.Gallon, nameof(AmountUnit.Gallon), MeasurementType.Volume, 0.0037854117840007m },
        };
    }


    public static TheoryData<AmountUnit> Keys()
    {
        var data = new TheoryData<AmountUnit>();
        foreach (var item in AmountUnit.Items)
        {
            data.Add(item);
        }
        return data;
    }
}
