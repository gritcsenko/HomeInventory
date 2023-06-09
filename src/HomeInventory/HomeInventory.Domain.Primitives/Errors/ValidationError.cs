namespace HomeInventory.Domain.Primitives.Errors;

public record ValidationError(string Message, IReadOnlyDictionary<string, object?> Metadata) : BaseError(Message, Metadata)
{
    public ValidationError(string Message)
        : this(Message, new Dictionary<string, object?>())
    {
    }
}
