using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class Money : ValueObject<Money>
{
    public Money(decimal amount, Currency currency)
        : base(amount, currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }

    public Currency Currency { get; }
}
