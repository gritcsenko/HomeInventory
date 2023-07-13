namespace HomeInventory.Domain.Primitives.Errors;

public record BaseError(string Message, IReadOnlyDictionary<string, object?> Metadata) : IError
{
    public BaseError(string message)
        : this(message, Dictionary.Empty<string, object?>())
    {
    }
}
