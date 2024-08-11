namespace HomeInventory.Domain.Errors;

public record InvalidCredentialsError() : Exceptional("Invalid credentials", -1_000_000_005);
