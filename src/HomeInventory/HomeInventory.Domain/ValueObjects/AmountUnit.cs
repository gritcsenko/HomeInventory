using System.Runtime.CompilerServices;
using HomeInventory.Core;
using HomeInventory.Domain.Primitives;
using Visus.Cuid;

namespace HomeInventory.Domain.ValueObjects;
public sealed class AmountUnit : BaseEnumeration<AmountUnit, Cuid>
{
    private readonly decimal _metricUnitScale = 1m;

    internal AmountUnit(MeasurementType measurement, [CallerMemberName] string name = "")
        : base(name, Cuid.NewCuid())
    {
        Measurement = measurement;
    }

    internal AmountUnit(AmountUnit baseUnit, decimal baseUnitScale, [CallerMemberName] string name = "")
        : base(name, baseUnit.Value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(baseUnitScale);

        Measurement = baseUnit.Measurement;

        _metricUnitScale = baseUnit._metricUnitScale * baseUnitScale;
        IsMetric = baseUnit.IsMetric && (baseUnitScale == 1 || baseUnitScale.IsPow10());
    }

    public static readonly AmountUnit Kelvin = new(MeasurementType.Temperature);

    public static readonly AmountUnit Piece = new(MeasurementType.Count);

    public static readonly AmountUnit CubicMeter = new(MeasurementType.Volume);
    public static readonly AmountUnit Liter = new(CubicMeter, 0.001M);
    public static readonly AmountUnit MilliLiter = new(Liter, 0.001M);
    public static readonly AmountUnit MicroLiter = new(MilliLiter, 0.001M);

    public static readonly AmountUnit Minim = new(MicroLiter, 61.611519921875M);
    public static readonly AmountUnit FluidDram = new(Minim, 60M);
    public static readonly AmountUnit Teaspoon = new(Minim, 80M);
    public static readonly AmountUnit Tablespoon = new(Teaspoon, 3M);
    public static readonly AmountUnit FluidOunce = new(Tablespoon, 2M);
    public static readonly AmountUnit Shot = new(Tablespoon, 3M);
    public static readonly AmountUnit Gill = new(FluidOunce, 4M);
    public static readonly AmountUnit Cup = new(Gill, 2M);
    public static readonly AmountUnit Pint = new(Cup, 2M);
    public static readonly AmountUnit Quart = new(Pint, 2M);
    public static readonly AmountUnit Pottle = new(Quart, 2M);
    public static readonly AmountUnit Gallon = new(Pottle, 2M); // 0.0037854117840007 CubicMeter

    public MeasurementType Measurement { get; }

    public bool IsMetric { get; } = true;

    public AmountUnit ToMetric() => Items.First(u => u.IsMetric && u.Measurement == Measurement && u._metricUnitScale == 1m);

    public (decimal Value, AmountUnit Unit) ToMetric(decimal value) => (value * _metricUnitScale, ToMetric());

    public override string ToString() => Name;
}