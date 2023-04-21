namespace HomeInventory.Domain.Primitives.Errors;

public record NotFoundError(string Message) : Error(Message);
