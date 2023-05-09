using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Errors;

public record DuplicateEmailError() : ConflictError("Duplicate email");
