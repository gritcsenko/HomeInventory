using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Domain.ValueObjects;

[UnitTest]
public class MeasurementTypeTests : BaseTest
{
    private static readonly EnumerationItemsCollection<MeasurementType> _items = EnumerationItemsCollection.CreateFor<MeasurementType>();

    [Fact]
    public void Items_Should_NotBeEmpty()
    {
        _items.Should().NotBeEmpty();
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
        var dictionary = _items.ToDictionary(x => x, x => x.Name);

        var actual = dictionary.ContainsKey(sut);

        actual.Should().BeTrue();
    }

    public static TheoryData<MeasurementType, string> Data() =>
        new()
        {
            { MeasurementType.Count, nameof(MeasurementType.Count) },
            { MeasurementType.Length, nameof(MeasurementType.Length) },
            { MeasurementType.Area, nameof(MeasurementType.Area) },
            { MeasurementType.Volume, nameof(MeasurementType.Volume) },
            { MeasurementType.Weight, nameof(MeasurementType.Weight) },
            { MeasurementType.Temperature, nameof(MeasurementType.Temperature) },
        };

    public static TheoryData<MeasurementType> Keys()
    {
        var data = new TheoryData<MeasurementType>();
        foreach (var item in _items)
        {
            data.Add(item);
        }
        return data;
    }
}
