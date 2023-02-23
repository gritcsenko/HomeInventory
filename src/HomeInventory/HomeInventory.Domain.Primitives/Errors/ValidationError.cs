namespace HomeInventory.Domain.Errors;

public record ValidationError(string Message, IReadOnlyDictionary<string, object?> Metadata) : Error(Message, Metadata)
{
    public ValidationError(string Message)
        : this(Message, new Dictionary<string, object?>())
    {
    }
}
