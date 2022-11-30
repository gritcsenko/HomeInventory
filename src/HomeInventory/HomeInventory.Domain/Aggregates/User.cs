using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates;

public class User : AggregateRoot<User, UserId>
{
    public User(UserId id)
        : base(id)
    {
    }

    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public Email Email { get; init; } = null!;

    public string Password { get; init; } = null!;
}
