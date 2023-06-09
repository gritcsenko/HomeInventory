namespace HomeInventory.Domain.Primitives.Errors;

public record NotFoundError(string Message) : BaseError(Message);
