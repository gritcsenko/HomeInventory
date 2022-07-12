namespace HomeInventory.Domain.Entities;
public class User
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
