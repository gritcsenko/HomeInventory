namespace HomeInventory.Domain.Primitives.Errors;

public record ObjectValidationError<TValue>(TValue Value) : ValidationError("Validation failed", new Dictionary<string, object?> { [nameof(Value)] = Value })
{
    public ObjectValidationError(ISupplier<TValue> supplier)
        : this(supplier.Invoke())
    {
    }
};
