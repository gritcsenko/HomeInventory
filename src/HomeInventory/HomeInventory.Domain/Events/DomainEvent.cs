using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Events;

public record DomainEvent(Guid Id, DateTimeOffset Created) : IDomainEvent;
