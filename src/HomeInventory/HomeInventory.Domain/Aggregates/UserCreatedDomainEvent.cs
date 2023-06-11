namespace HomeInventory.Domain.Aggregates;

public sealed record UserCreatedDomainEvent : DomainEvent
{
    public UserCreatedDomainEvent(DateTimeOffset created, User user)
        : base(Guid.NewGuid(), created)
    {
        User = user;
    }

    public User User { get; }
}
