using System.Runtime.CompilerServices;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.ValueObjects;
public sealed class AmountUnit : BaseEnumeration<AmountUnit, Ulid>
{
    private AmountUnit(string name, IIdSupplier<Ulid> supplier, MeasurementType measurement)
        : base(name, supplier.Supply())
    {
        Measurement = measurement;
    }

    public static readonly AmountUnit Kelvin = Create(MeasurementType.Temperature);
    public static readonly AmountUnit Celsius = Create(Kelvin, static x => x - 272.15M);

    public static readonly AmountUnit Piece = Create(MeasurementType.Count);

    public static readonly AmountUnit CubicMeter = Create(MeasurementType.Volume);
    public static readonly AmountUnit Liter = Create(CubicMeter, static x => x * 0.001M);
    public static readonly AmountUnit MilliLiter = Create(Liter, static x => x * 0.001M);
    public static readonly AmountUnit MicroLiter = Create(MilliLiter, static x => x * 0.001M);

    public static readonly AmountUnit Minim = Create(MicroLiter, static x => x * 61.611519921875M);
    public static readonly AmountUnit FluidDram = Create(Minim, static x => x * 60M);
    public static readonly AmountUnit Teaspoon = Create(Minim, static x => x * 80M);
    public static readonly AmountUnit Tablespoon = Create(Teaspoon, static x => x * 3M);
    public static readonly AmountUnit FluidOunce = Create(Tablespoon, static x => x * 2M);
    public static readonly AmountUnit Shot = Create(Tablespoon, static x => x * 3M);
    public static readonly AmountUnit Gill = Create(FluidOunce, static x => x * 4M);
    public static readonly AmountUnit Cup = Create(Gill, static x => x * 2M);
    public static readonly AmountUnit Pint = Create(Cup, static x => x * 2M);
    public static readonly AmountUnit Quart = Create(Pint, static x => x * 2M);
    public static readonly AmountUnit Pottle = Create(Quart, static x => x * 2M);
    public static readonly AmountUnit Gallon = Create(Pottle, static x => x * 2M); // 0.0037854117840007 CubicMeter

    public MeasurementType Measurement { get; }

    internal static AmountUnit Create(MeasurementType measurement, [CallerMemberName] string name = "") => Create(measurement, IdSuppliers.Ulid, name);

    internal static AmountUnit Create(MeasurementType measurement, IIdSupplier<Ulid> supplier, [CallerMemberName] string name = "") => new(name, supplier, measurement);

    internal static AmountUnit Create(AmountUnit baseUnit, Func<decimal, decimal> fromBase, [CallerMemberName] string name = "") => Create(baseUnit, fromBase, IdSuppliers.Ulid, name);

    internal static AmountUnit Create(AmountUnit baseUnit, Func<decimal, decimal> _, IIdSupplier<Ulid> supplier, [CallerMemberName] string name = "")
    {
        return new(
            name,
            supplier,
            baseUnit.Measurement);
    }
}
