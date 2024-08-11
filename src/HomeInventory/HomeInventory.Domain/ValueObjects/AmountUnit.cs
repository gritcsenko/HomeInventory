using System.Runtime.CompilerServices;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.ValueObjects;
public sealed class AmountUnit : BaseEnumeration<AmountUnit, Ulid>
{
    private readonly decimal _metricUnitScale;

    private AmountUnit(string name, IIdSupplier<Ulid> supplier, MeasurementType measurement, decimal metricUnitScale, bool isMetric)
        : base(name, supplier.Supply())
    {
        Measurement = measurement;
        _metricUnitScale = metricUnitScale;
        IsMetric = isMetric;
    }

    public static readonly AmountUnit Kelvin = Create(MeasurementType.Temperature);

    public static readonly AmountUnit Piece = Create(MeasurementType.Count);

    public static readonly AmountUnit CubicMeter = Create(MeasurementType.Volume);
    public static readonly AmountUnit Liter = Create(CubicMeter, 0.001M);
    public static readonly AmountUnit MilliLiter = Create(Liter, 0.001M);
    public static readonly AmountUnit MicroLiter = Create(MilliLiter, 0.001M);

    public static readonly AmountUnit Minim = Create(MicroLiter, 61.611519921875M);
    public static readonly AmountUnit FluidDram = Create(Minim, 60M);
    public static readonly AmountUnit Teaspoon = Create(Minim, 80M);
    public static readonly AmountUnit Tablespoon = Create(Teaspoon, 3M);
    public static readonly AmountUnit FluidOunce = Create(Tablespoon, 2M);
    public static readonly AmountUnit Shot = Create(Tablespoon, 3M);
    public static readonly AmountUnit Gill = Create(FluidOunce, 4M);
    public static readonly AmountUnit Cup = Create(Gill, 2M);
    public static readonly AmountUnit Pint = Create(Cup, 2M);
    public static readonly AmountUnit Quart = Create(Pint, 2M);
    public static readonly AmountUnit Pottle = Create(Quart, 2M);
    public static readonly AmountUnit Gallon = Create(Pottle, 2M); // 0.0037854117840007 CubicMeter

    public MeasurementType Measurement { get; }

    public bool IsMetric { get; }

    public bool IsScaled => _metricUnitScale != 1m;

    public AmountUnit ToMetric() => Items.First(u => u.IsMetric && u.Measurement == Measurement && !u.IsScaled);

    public (decimal Value, AmountUnit Unit) ToMetric(decimal value) => (value * _metricUnitScale, ToMetric());

    public override string ToString() => Name;

    internal static AmountUnit Create(MeasurementType measurement, [CallerMemberName] string name = "") => Create(measurement, IdSuppliers.Ulid, name);

    internal static AmountUnit Create(AmountUnit baseUnit, decimal baseUnitScale, [CallerMemberName] string name = "") => Create(baseUnit, baseUnitScale, IdSuppliers.Ulid, name);

    internal static AmountUnit Create(MeasurementType measurement, IIdSupplier<Ulid> supplier, [CallerMemberName] string name = "") => new(name, supplier, measurement, 1m, true);

    internal static AmountUnit Create(AmountUnit baseUnit, decimal baseUnitScale, IIdSupplier<Ulid> supplier, [CallerMemberName] string name = "")
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(baseUnitScale);
        return new(
            name,
            supplier,
            baseUnit.Measurement,
            baseUnit._metricUnitScale * baseUnitScale,
            baseUnit.IsMetric && (baseUnitScale == 1 || baseUnitScale.IsPow10()));
    }
}
