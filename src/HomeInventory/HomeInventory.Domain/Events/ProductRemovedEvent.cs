using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Events;

public record ProductRemovedEvent(IAggregateRoot Source, ProductId ProductId) : IDomainEvent;

