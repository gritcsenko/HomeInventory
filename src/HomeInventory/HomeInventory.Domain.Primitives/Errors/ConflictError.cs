namespace HomeInventory.Domain.Errors;

public record ConflictError(string Message) : Error(Message);
