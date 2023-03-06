using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates;

public class User : AggregateRoot<User, UserId>
{
    public User(UserId id)
        : base(id)
    {
    }

    public Email Email { get; init; } = null!;

    public string Password { get; init; } = null!;
}

