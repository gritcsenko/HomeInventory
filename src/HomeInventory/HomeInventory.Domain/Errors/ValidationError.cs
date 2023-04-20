using FluentResults;

namespace HomeInventory.Domain.Errors;

public class ValidationError : Error
{
    public ValidationError(string message)
        : base(message)
    {
    }
}
