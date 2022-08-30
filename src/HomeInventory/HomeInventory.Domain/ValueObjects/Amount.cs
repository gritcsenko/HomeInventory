using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public class Amount : ValueObject<Amount>
{
    internal Amount(decimal value, AmountUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public decimal Value { get; }

    public AmountUnit Unit { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }
}
