using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.UserManagement.Errors;

public record DuplicateEmailError() : ConflictError(DefaultMessage)
{
    public static readonly string DefaultMessage = "Duplicate email";

    public static readonly Error Instance = new DuplicateEmailError();
};
