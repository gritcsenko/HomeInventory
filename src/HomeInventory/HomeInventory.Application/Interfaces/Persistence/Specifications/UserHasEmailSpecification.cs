using System.Linq.Expressions;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

internal class UserHasEmailSpecification : FilterSpecification<User>
{
    private readonly string _email;

    public UserHasEmailSpecification(string email) => _email = email;

    protected override Expression<Func<User, bool>> ToExpression() => x => x.Email.Equals(_email);
}
