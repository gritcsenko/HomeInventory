using DotNext;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using Visus.Cuid;

namespace HomeInventory.Domain.Aggregates;

public class User(UserId id) : AggregateRoot<User, UserId>(id)
{
    public required Email Email { get; init; }

    public required string Password { get; init; }

    public void OnUserCreated(ISupplier<Cuid> supplier, TimeProvider dateTimeService) =>
        AddDomainEvent(new UserCreatedDomainEvent(supplier, dateTimeService, this));
}
