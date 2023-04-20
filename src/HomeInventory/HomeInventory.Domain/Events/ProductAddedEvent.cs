using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Events;

public record ProductAddedEvent(IAggregateRoot Source, ProductId ProductId) : IDomainEvent;

