using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.UserManagement.Errors;

public record DuplicateEmailError() : ConflictError(DefaultMessage)
{
    public static readonly string DefaultMessage = "Duplicate email";
    
    public static Error Instance { get; } = new DuplicateEmailError();
};
