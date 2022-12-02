namespace HomeInventory.Domain.Errors;

public class ObjectValidationError<TValue> : ValidationError
{
    public ObjectValidationError(TValue value)
        : base("Validation failed")
    {
        Value = value;
        WithMetadata(nameof(value), value);
    }

    public TValue Value { get; }
}
