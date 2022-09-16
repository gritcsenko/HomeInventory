using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class AmountUnit : Enumeration<AmountUnit, Guid>
{
    internal AmountUnit(string name, MeasurementType measurement)
        : base(name, Guid.NewGuid())
    {
        Measurement = measurement;
    }

    internal AmountUnit(string name, AmountUnit baseUnit, decimal ciUnitFactor)
        : base(name, baseUnit.Value)
    {
        Measurement = baseUnit.Measurement;
        CIUnitFactor = ciUnitFactor;
    }

    public static readonly AmountUnit Piece = new(nameof(Piece), MeasurementType.Count);
    public static readonly AmountUnit CubicMeter = new(nameof(CubicMeter), MeasurementType.Volume);
    public static readonly AmountUnit Gallon = new(nameof(Gallon), CubicMeter, 0.0037854117840007m);

    public MeasurementType Measurement { get; }

    public decimal CIUnitFactor { get; } = 1m;
}
