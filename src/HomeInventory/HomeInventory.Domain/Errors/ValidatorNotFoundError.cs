using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Errors;

public record ValidatorNotFoundError : NotFoundError
{
    public ValidatorNotFoundError(AmountUnit unit)
        : base("Validator not found")
    {
        Unit = unit;
    }

    public AmountUnit Unit { get; }
}
