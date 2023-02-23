namespace HomeInventory.Domain.Errors;

public interface IError
{
    string Message { get; }

    IReadOnlyDictionary<string, object?> Metadata { get; }
}
