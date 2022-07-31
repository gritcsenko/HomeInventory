using FluentAssertions;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Domain.ValueObjects;

[Trait("Category", "Unit")]
public class MeasurementTypeTests : BaseTest
{
    [Theory]
    [MemberData(nameof(Data))]
    public void PropertiesShouldMatch(MeasurementType sut, string name, int value)
    {
        sut.Name.Should().Be(name);
        sut.Value.Should().Be(value);
    }

    public static TheoryData<MeasurementType, string, int> Data()
    {
        return new()
        {
            { MeasurementType.Count, nameof(MeasurementType.Count), 0 },
            { MeasurementType.Length, nameof(MeasurementType.Length), 1 },
            { MeasurementType.Area, nameof(MeasurementType.Area), 2 },
            { MeasurementType.Volume, nameof(MeasurementType.Volume), 3 },
            { MeasurementType.Weight, nameof(MeasurementType.Weight), 4 },
            { MeasurementType.Temperature, nameof(MeasurementType.Temperature), 5 },
        };
    }
}
