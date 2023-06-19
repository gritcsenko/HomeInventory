﻿using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates;

public class User : AggregateRoot<User, UserId>
{
    public User(UserId id)
        : base(id)
    {
    }

    public required Email Email { get; init; }

    public required string Password { get; init; }

    public void OnUserCreated(IDateTimeService dateTimeService) =>
        AddDomainEvent(new UserCreatedDomainEvent(dateTimeService.UtcNow, this));
}
