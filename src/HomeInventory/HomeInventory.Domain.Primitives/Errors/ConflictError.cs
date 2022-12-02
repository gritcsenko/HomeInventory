using FluentResults;

namespace HomeInventory.Domain.Errors;

public class ConflictError : Error
{
    public ConflictError(string message)
        : base(message)
    {
    }
}
