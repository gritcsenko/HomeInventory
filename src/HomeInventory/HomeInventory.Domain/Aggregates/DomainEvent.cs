using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Aggregates;

public record DomainEvent(Guid Id, DateTimeOffset Created) : IDomainEvent;
