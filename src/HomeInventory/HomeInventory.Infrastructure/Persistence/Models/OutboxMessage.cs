namespace HomeInventory.Infrastructure.Persistence.Models;

public record OutboxMessage(Guid Id, DateTimeOffset OccurredOn, string Type, string Content) : IPersistentModel;
