using FluentResults;

namespace HomeInventory.Domain.Errors;

public class InvalidCredentialsError : Error
{
    public InvalidCredentialsError()
        : base("Invalid credentials")
    {
    }
}
