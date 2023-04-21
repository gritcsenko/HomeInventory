using FluentResults;

namespace HomeInventory.Domain.Primitives.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string message)
        : base(message)
    {
    }
}
