using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public class Amount : ValueObject<Amount>
{
    private readonly decimal _value;
    private readonly AmountUnit _unit;

    internal Amount(decimal value, AmountUnit unit)
    {
        _value = value;
        _unit = unit;
    }

    public decimal Value => _value;

    public AmountUnit Unit => _unit;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
        yield return _unit;
    }
}
