using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public class CreateUserSpecification : ICreateEntitySpecification<User>
{
    public CreateUserSpecification(string firstName, string lastName, Email email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public Email Email { get; }
    public string Password { get; }
}
