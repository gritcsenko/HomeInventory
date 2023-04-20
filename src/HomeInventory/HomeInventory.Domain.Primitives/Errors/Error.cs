namespace HomeInventory.Domain.Errors;

public record Error(string Message, IReadOnlyDictionary<string, object?> Metadata) : IError
{
    public Error(string message)
        : this(message, new Dictionary<string, object?>())
    {
    }
}
