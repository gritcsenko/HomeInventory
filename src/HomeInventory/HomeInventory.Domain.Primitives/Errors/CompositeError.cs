using FluentResults;

namespace HomeInventory.Domain.Errors;

public class CompositeError : Error
{
    public CompositeError(IEnumerable<IError> errors)
    {
        CausedBy(errors);
    }

    public static Error Create<TError>(IEnumerable<TError> errors)
        where TError : Error
    {
        if (errors is IReadOnlyCollection<TError> collection && collection.Count == 1)
        {
            return collection.First();
        }

        return new CompositeError(errors);
    }
}
