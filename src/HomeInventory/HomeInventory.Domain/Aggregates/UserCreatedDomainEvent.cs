using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Aggregates;

public sealed record UserCreatedDomainEvent(Guid Id, DateTimeOffset Created, User User) : IDomainEvent
{
    public UserCreatedDomainEvent(DateTimeOffset created, User user)
        : this(Guid.NewGuid(), created, user)
    {
    }

    IAggregateRoot IDomainEvent.Source => User;
}
