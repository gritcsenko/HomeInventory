using HomeInventory.Domain.Aggregates;

namespace HomeInventory.Domain.Events;

public sealed record UserCreatedDomainEvent : DomainEvent
{
    public UserCreatedDomainEvent(DateTimeOffset created, User user)
        : base(Guid.NewGuid(), created)
    {
        User = user;
    }

    public User User { get; }
}
