using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Domain.ValueObjects;

[Trait("Category", "Unit")]
public class MeasurementTypeTests : BaseTest
{
    [Fact]
    public void Items_Should_NotBeEmpty()
    {
        MeasurementType.Items.Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void PropertiesShouldMatch(MeasurementType sut, string name)
    {
        sut.Name.Should().Be(name);
    }

    [Theory]
    [MemberData(nameof(Keys))]
    public void CanBeUsedAsDictionaryKey(MeasurementType sut)
    {
        var dictionary = MeasurementType.Items.ToDictionary(x => x, x => x.Name);

        var actual = dictionary.ContainsKey(sut);

        actual.Should().BeTrue();
    }

    public static TheoryData<MeasurementType, string> Data()
    {
        return new()
        {
            { MeasurementType.Count, nameof(MeasurementType.Count) },
            { MeasurementType.Length, nameof(MeasurementType.Length) },
            { MeasurementType.Area, nameof(MeasurementType.Area) },
            { MeasurementType.Volume, nameof(MeasurementType.Volume) },
            { MeasurementType.Weight, nameof(MeasurementType.Weight) },
            { MeasurementType.Temperature, nameof(MeasurementType.Temperature) },
        };
    }

    public static TheoryData<MeasurementType> Keys()
    {
        var data = new TheoryData<MeasurementType>();
        foreach (var item in MeasurementType.Items)
        {
            data.Add(item);
        }
        return data;
    }
}
