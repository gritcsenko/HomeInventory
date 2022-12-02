using FluentResults;

namespace HomeInventory.Domain.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string message)
        : base(message)
    {
    }
}
