namespace HomeInventory.Domain.Errors;

public class ObjectValidationError<TValue> : ValidationError
{
    public ObjectValidationError(TValue value)
        : base("Validation failed")
    {
        WithMetadata(nameof(value), value);
    }
}
