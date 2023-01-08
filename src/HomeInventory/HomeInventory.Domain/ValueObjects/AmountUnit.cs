using HomeInventory.Domain.Primitives;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Domain.ValueObjects;

public sealed class AmountUnit : Enumeration<AmountUnit, Guid>
{
    internal AmountUnit(string name, MeasurementType measurement)
        : base(name, Guid.NewGuid())
    {
        Measurement = measurement;
        MetricUnit = this;
    }

    private AmountUnit(string name, AmountUnit baseUnit, decimal baseUnitFactor)
        : base(name, baseUnit.Value)
    {
        MetricUnit = baseUnit.MetricUnit;
        Measurement = baseUnit.Measurement;
        MetricUnitFactor = baseUnit.MetricUnitFactor * baseUnitFactor;
    }

    public static readonly AmountUnit Piece = new(nameof(Piece), MeasurementType.Count);

    public static readonly AmountUnit CubicMeter = new(nameof(CubicMeter), MeasurementType.Volume);
    public static readonly AmountUnit Liter = new(nameof(Liter), CubicMeter, 0.001m);
    public static readonly AmountUnit MilliLiter = new(nameof(MilliLiter), Liter, 0.001m);
    public static readonly AmountUnit MicroLiter = new(nameof(MicroLiter), MilliLiter, 0.001m);

    public static readonly AmountUnit Minim = new(nameof(Minim), MicroLiter, 61.611519921875m);
    public static readonly AmountUnit FluidDram = new(nameof(FluidDram), Minim, 60m);
    public static readonly AmountUnit Teaspoon = new(nameof(Teaspoon), Minim, 80m);
    public static readonly AmountUnit Tablespoon = new(nameof(Tablespoon), Teaspoon, 3m);
    public static readonly AmountUnit FluidOunce = new(nameof(FluidOunce), Tablespoon, 2m);
    public static readonly AmountUnit Shot = new(nameof(Shot), Tablespoon, 3m);
    public static readonly AmountUnit Gill = new(nameof(Gill), FluidOunce, 4m);
    public static readonly AmountUnit Cup = new(nameof(Cup), Gill, 2m);
    public static readonly AmountUnit Pint = new(nameof(Pint), Cup, 2m);
    public static readonly AmountUnit Quart = new(nameof(Quart), Pint, 2m);
    public static readonly AmountUnit Pottle = new(nameof(Pottle), Quart, 2m);
    public static readonly AmountUnit Gallon = new(nameof(Gallon), Pottle, 2m); // 0.0037854117840007 CubicMeter

    public AmountUnit MetricUnit { get; }

    public MeasurementType Measurement { get; }

    public decimal MetricUnitFactor { get; } = 1m;

    public bool IsMetric => MetricUnitFactor == 1m;

    public decimal ToMetric(decimal value) => value * MetricUnitFactor;

    public static OneOf<AmountUnit, NotFound> TryParse(string name)
    {
        foreach (var item in Items)
        {
            if (item.Name == name)
                return item;
        }

        return new NotFound();
    }
}
