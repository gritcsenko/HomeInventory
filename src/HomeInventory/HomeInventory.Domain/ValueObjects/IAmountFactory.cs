using FluentResults;

namespace HomeInventory.Domain.ValueObjects;

public interface IAmountFactory
{
    Result<Amount> Create(decimal value, AmountUnit unit);
}
