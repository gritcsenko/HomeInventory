using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.UserManagement.Events;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Domain.UserManagement.Aggregates;

public class User(UserId id) : AggregateRoot<User, UserId>(id)
{
    public required Email Email { get; init; }

    public required string Password { get; init; }

    public void OnUserCreated(IIdSupplier<Ulid> supplier, TimeProvider dateTimeService) =>
        AddDomainEvent(new UserCreatedDomainEvent(supplier, dateTimeService, this));
}
