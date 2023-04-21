using FluentResults;

namespace HomeInventory.Domain.Primitives.Errors;

public class ConflictError : Error
{
    public ConflictError(string message)
        : base(message)
    {
    }
}
