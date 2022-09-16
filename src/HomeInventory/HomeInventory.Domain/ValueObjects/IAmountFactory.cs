using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;

public interface IAmountFactory
{
    ErrorOr<Amount> Create(decimal value, AmountUnit unit);
}
