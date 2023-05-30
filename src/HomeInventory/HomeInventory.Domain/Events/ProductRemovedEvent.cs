using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Events;

public record ProductRemovedEvent(Guid Id, DateTimeOffset Created, IAggregateRoot Source, ProductId ProductId) : IDomainEvent;

