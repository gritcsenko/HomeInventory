using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;
public class User : AggregateRoot<User, UserId>
{
    public User(UserId id)
        : base(id)
    {
    }

    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}

