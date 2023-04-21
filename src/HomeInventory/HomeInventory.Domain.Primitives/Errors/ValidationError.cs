using FluentResults;

namespace HomeInventory.Domain.Primitives.Errors;

public class ValidationError : Error
{
    public ValidationError(string message)
        : base(message)
    {
    }
}
