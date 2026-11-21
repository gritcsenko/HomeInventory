namespace HomeInventory.Domain.ValueObjects;

public interface IAmountFactory
{
    Validated<Amount, Error> Create(decimal value, AmountUnit unit);
}
