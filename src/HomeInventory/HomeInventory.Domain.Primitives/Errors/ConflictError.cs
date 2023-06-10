namespace HomeInventory.Domain.Primitives.Errors;

public record ConflictError(string Message) : BaseError(Message);
