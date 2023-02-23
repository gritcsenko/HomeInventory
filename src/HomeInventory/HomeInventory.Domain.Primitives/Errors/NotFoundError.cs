namespace HomeInventory.Domain.Errors;

public record NotFoundError(string Message) : Error(Message);
