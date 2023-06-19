using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Events;

public record DomainEvent(Ulid Id, DateTimeOffset Created) : IDomainEvent;
