namespace HomeInventory.Domain.Errors;

public record ObjectValidationError<TValue>(TValue Value) : ValidationError("Validation failed", new Dictionary<string, object?> { [nameof(Value)] = Value });
