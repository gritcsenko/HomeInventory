using FluentResults;

namespace HomeInventory.Domain.Errors;

public class UserCreationError : Error
{
    public UserCreationError()
        : base("Failed to create new user")
    {
    }
}
