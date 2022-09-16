using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;

public interface IAmountFactory
{
    ErrorOr<Amount> Pieces(int value);
}
