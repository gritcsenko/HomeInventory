using HomeInventory.Domain.Aggregates;

namespace HomeInventory.Domain.Events;

public sealed record UserCreatedDomainEvent : DomainEvent
{
    public UserCreatedDomainEvent(DateTimeOffset created, User user)
        : this(Ulid.NewUlid(), created, user)
    {
    }

    public UserCreatedDomainEvent(Ulid id, DateTimeOffset created, User user)
        : base(id, created)
    {
        User = user;
    }

    public User User { get; }
}
