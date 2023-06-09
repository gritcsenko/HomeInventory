using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Errors;

public record InvalidCredentialsError() : BaseError("Invalid credentials");
