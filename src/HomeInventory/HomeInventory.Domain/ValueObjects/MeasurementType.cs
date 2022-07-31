using Ardalis.SmartEnum;

namespace HomeInventory.Domain.ValueObjects;

public class MeasurementType : SmartEnum<MeasurementType>
{
    private MeasurementType(string name, int value)
        : base(name, value)
    {
    }

    public static MeasurementType Count { get; } = new MeasurementType(nameof(Count), 0);
    public static MeasurementType Length { get; } = new MeasurementType(nameof(Length), 1);
    public static MeasurementType Area { get; } = new MeasurementType(nameof(Area), 2);
    public static MeasurementType Volume { get; } = new MeasurementType(nameof(Volume), 3);
    public static MeasurementType Weight { get; } = new MeasurementType(nameof(Weight), 4);
    public static MeasurementType Temperature { get; } = new MeasurementType(nameof(Temperature), 5);
}
