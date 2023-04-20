using HomeInventory.Domain.Primitives;

namespace HomeInventory.Infrastructure.Persistence.Models;

public record OutboxMessage(Guid Id, DateTimeOffset OccurredOn, IDomainEvent Content) : IPersistentModel;
