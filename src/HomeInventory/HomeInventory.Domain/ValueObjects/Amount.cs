using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class Amount : ValueObject<Amount>
{
    internal Amount(decimal value, AmountUnit unit)
        : base(value, unit)
    {
        Value = value;
        Unit = unit;
    }

    public decimal Value { get; }

    public AmountUnit Unit { get; }

    public Amount ToMetric() => Unit.IsMetric ? this : new Amount(Unit.ToMetric(Value), Unit.MetricUnit);
}
