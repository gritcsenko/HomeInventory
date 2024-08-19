namespace HomeInventory.Domain.Primitives.Errors;

public record ConflictError(string Message) : Exceptional(Message, -1_000_000_003);
