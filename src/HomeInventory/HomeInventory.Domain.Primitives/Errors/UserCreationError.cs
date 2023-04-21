using FluentResults;

namespace HomeInventory.Domain.Primitives.Errors;

public class UserCreationError : Error
{
    public UserCreationError()
        : base("Failed to create new user")
    {
    }
}
