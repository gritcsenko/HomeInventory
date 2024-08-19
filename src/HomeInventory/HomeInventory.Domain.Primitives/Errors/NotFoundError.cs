namespace HomeInventory.Domain.Primitives.Errors;

public record NotFoundError(string Message) : Exceptional(Message, -1_000_000_002);
