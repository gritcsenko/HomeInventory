using Ardalis.SmartEnum;

namespace HomeInventory.Domain.ValueObjects;

public sealed class AmountUnit : SmartEnum<AmountUnit>
{
    private AmountUnit(string name, MeasurementType measurement, int value, decimal standardUnitFactor = 1m)
        : base(name, value)
    {
        Measurement = measurement;
        StandardUnitFactor = standardUnitFactor;
    }

    public MeasurementType Measurement { get; }

    public decimal StandardUnitFactor { get; }

    public static AmountUnit Pieces { get; } = new AmountUnit(nameof(Pieces), MeasurementType.Count, 0);
}
