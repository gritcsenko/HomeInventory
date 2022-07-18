using HomeInventory.Domain.Entities;
using System.Linq.Expressions;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public class UserHasEmailSpecification : FilterSpecification<User>
{
    public UserHasEmailSpecification(string email) => Email = email;

    public string Email { get; }

    protected override Expression<Func<User, bool>> ToExpression() => x => x.Email.Equals(Email);
}
