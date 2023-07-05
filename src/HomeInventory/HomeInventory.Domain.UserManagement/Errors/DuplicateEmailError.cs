using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Errors;

public record DuplicateEmailError() : ConflictError(DefaultMessage)
{
    public static readonly string DefaultMessage = "Duplicate email";
};
