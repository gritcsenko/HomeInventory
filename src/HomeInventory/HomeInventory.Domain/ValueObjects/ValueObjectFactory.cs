using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;

public abstract class ValueObjectFactory<TObject>
    where TObject : notnull, ValueObject<TObject>
{
    protected ErrorOr<TObject> GetValidationError() => GetValidationErrorCore();

    protected virtual Error GetValidationErrorCore() => Error.Validation();
}
