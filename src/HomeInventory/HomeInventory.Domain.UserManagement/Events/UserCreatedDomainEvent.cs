using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.UserManagement.Aggregates;

namespace HomeInventory.Domain.UserManagement.Events;

public sealed record UserCreatedDomainEvent : DomainEvent
{
    public UserCreatedDomainEvent(IIdSupplier<Ulid> supplier, TimeProvider dateTimeService, User user)
        : base(supplier, dateTimeService) =>
        User = user;

    public User User { get; }
}
