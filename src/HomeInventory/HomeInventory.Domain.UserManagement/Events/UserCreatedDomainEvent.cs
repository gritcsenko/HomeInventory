using DotNext;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Domain.Events;

public sealed record UserCreatedDomainEvent : DomainEvent
{
    public UserCreatedDomainEvent(ISupplier<Ulid> supplier, TimeProvider dateTimeService, User user)
        : base(supplier, dateTimeService) =>
        User = user;

    public User User { get; }
}
