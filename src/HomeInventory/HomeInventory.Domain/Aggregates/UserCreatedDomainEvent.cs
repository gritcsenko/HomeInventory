using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Aggregates;

public record UserCreatedDomainEvent(DateTimeOffset Created, User User) : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    IAggregateRoot IDomainEvent.Source => User;
}
