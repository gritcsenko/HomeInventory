namespace HomeInventory.Domain.ValueObjects;

public class MeasurementType : Enumeration<MeasurementType, Guid>
{
    private MeasurementType(string name)
        : base(name, Guid.NewGuid())
    {
    }

    public static MeasurementType Count { get; } = new MeasurementType(nameof(Count));
    public static MeasurementType Length { get; } = new MeasurementType(nameof(Length));
    public static MeasurementType Area { get; } = new MeasurementType(nameof(Area));
    public static MeasurementType Volume { get; } = new MeasurementType(nameof(Volume));
    public static MeasurementType Weight { get; } = new MeasurementType(nameof(Weight));
    public static MeasurementType Temperature { get; } = new MeasurementType(nameof(Temperature));
}
