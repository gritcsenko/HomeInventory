using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Events;

public record ProductAddedEvent(IAggregateRoot Source, Product Product) : IDomainEvent;

