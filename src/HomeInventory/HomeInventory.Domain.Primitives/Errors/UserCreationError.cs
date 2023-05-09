namespace HomeInventory.Domain.Primitives.Errors;

public record UserCreationError : Error
{
    public UserCreationError()
        : base("Failed to create new user")
    {
    }
}
