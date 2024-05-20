using HomeInventory.Domain.Aggregates;

namespace HomeInventory.Domain.Events;

public sealed record UserCreatedDomainEvent : DomainEvent
{
    public UserCreatedDomainEvent(TimeProvider dateTimeService, User user)
        : base(dateTimeService) =>
        User = user;

    public User User { get; }
}
