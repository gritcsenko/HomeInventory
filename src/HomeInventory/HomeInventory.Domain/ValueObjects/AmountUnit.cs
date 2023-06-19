using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class AmountUnit : BaseEnumeration<AmountUnit, Ulid>
{
    private readonly decimal _metricUnitScale = 1m;

    internal AmountUnit(string name, MeasurementType measurement)
        : base(name, Ulid.NewUlid())
    {
        Measurement = measurement;
        MetricUnit = this;
    }

    private AmountUnit(string name, AmountUnit baseUnit, decimal baseUnitScale)
        : base(name, baseUnit.Value)
    {
        MetricUnit = baseUnit.MetricUnit;
        Measurement = baseUnit.Measurement;

        _metricUnitScale = baseUnit._metricUnitScale * baseUnitScale;
        var power = Math.Log10(decimal.ToDouble(_metricUnitScale));
        IsMetric = double.IsFinite(power) && Math.Round(power) == power;
    }

    public static readonly AmountUnit Kelvin = new(nameof(Kelvin), MeasurementType.Temperature);

    public static readonly AmountUnit Piece = new(nameof(Piece), MeasurementType.Count);

    public static readonly AmountUnit CubicMeter = new(nameof(CubicMeter), MeasurementType.Volume);
    public static readonly AmountUnit Liter = new(nameof(Liter), CubicMeter, 0.001M);
    public static readonly AmountUnit MilliLiter = new(nameof(MilliLiter), Liter, 0.001M);
    public static readonly AmountUnit MicroLiter = new(nameof(MicroLiter), MilliLiter, 0.001M);

    public static readonly AmountUnit Minim = new(nameof(Minim), MicroLiter, 61.611519921875M);
    public static readonly AmountUnit FluidDram = new(nameof(FluidDram), Minim, 60M);
    public static readonly AmountUnit Teaspoon = new(nameof(Teaspoon), Minim, 80M);
    public static readonly AmountUnit Tablespoon = new(nameof(Tablespoon), Teaspoon, 3M);
    public static readonly AmountUnit FluidOunce = new(nameof(FluidOunce), Tablespoon, 2M);
    public static readonly AmountUnit Shot = new(nameof(Shot), Tablespoon, 3M);
    public static readonly AmountUnit Gill = new(nameof(Gill), FluidOunce, 4M);
    public static readonly AmountUnit Cup = new(nameof(Cup), Gill, 2M);
    public static readonly AmountUnit Pint = new(nameof(Pint), Cup, 2M);
    public static readonly AmountUnit Quart = new(nameof(Quart), Pint, 2M);
    public static readonly AmountUnit Pottle = new(nameof(Pottle), Quart, 2M);
    public static readonly AmountUnit Gallon = new(nameof(Gallon), Pottle, 2M); // 0.0037854117840007 CubicMeter

    public AmountUnit MetricUnit { get; }

    public MeasurementType Measurement { get; }

    public bool IsMetric { get; } = true;

    public decimal ToMetric(decimal value) => value * _metricUnitScale;

    public override string ToString() => Name;
}
