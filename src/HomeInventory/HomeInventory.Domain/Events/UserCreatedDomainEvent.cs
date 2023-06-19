using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Events;

public sealed record UserCreatedDomainEvent : DomainEvent
{
    public UserCreatedDomainEvent(IDateTimeService dateTimeService, User user)
        : base(dateTimeService)
    {
        User = user;
    }

    public User User { get; }
}
