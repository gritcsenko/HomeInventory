namespace HomeInventory.Domain.ValueObjects;

public interface IAmountFactory
{
    Validation<Error, Amount> Create(decimal value, AmountUnit unit);
}
