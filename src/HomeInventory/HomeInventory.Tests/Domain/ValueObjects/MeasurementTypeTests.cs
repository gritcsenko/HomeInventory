using FluentAssertions;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Domain.ValueObjects;

[Trait("Category", "Unit")]
public class MeasurementTypeTests : BaseTest
{
    [Theory]
    [MemberData(nameof(Data))]
    public void PropertiesShouldMatch(MeasurementType sut, string name)
    {
        sut.Name.Should().Be(name);
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
}
