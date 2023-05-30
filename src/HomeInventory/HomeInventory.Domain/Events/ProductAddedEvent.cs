using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Events;

public record ProductAddedEvent(Guid Id, DateTimeOffset Created, IAggregateRoot Source, ProductId ProductId) : IDomainEvent;

