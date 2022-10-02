using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Errors;
public class ValidatorNotFoundError : NotFoundError
{
    public ValidatorNotFoundError(AmountUnit unit)
        : base("Validator not found")
    {
        WithMetadata(nameof(unit), unit);
    }
}
