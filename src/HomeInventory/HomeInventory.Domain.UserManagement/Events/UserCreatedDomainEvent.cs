using DotNext;
using HomeInventory.Domain.Aggregates;
using Visus.Cuid;

namespace HomeInventory.Domain.Events;

public sealed record UserCreatedDomainEvent : DomainEvent
{
    public UserCreatedDomainEvent(ISupplier<Cuid> supplier, TimeProvider dateTimeService, User user)
        : base(supplier, dateTimeService) =>
        User = user;

    public User User { get; }
}
