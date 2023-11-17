using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class Amount : ValueObject<Amount>
{
    internal Amount(decimal value, AmountUnit unit)
        : base(value, unit)
    {
    }

    public decimal Value => GetComponent<decimal>(0);

    public AmountUnit Unit => GetComponent<AmountUnit>(1);

    public Amount ToMetric() => Unit.IsMetric ? this : new Amount(Unit.ToMetric(Value), Unit.MetricUnit);
}
