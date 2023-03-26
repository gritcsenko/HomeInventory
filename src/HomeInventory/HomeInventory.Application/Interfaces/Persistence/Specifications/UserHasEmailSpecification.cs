using System.Linq.Expressions;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

internal class UserHasEmailSpecification : FilterSpecification<User>
{
    private readonly Email _email;

    public UserHasEmailSpecification(Email email) => _email = email;

    protected override Expression<Func<User, bool>> ToExpressionCore() => x => x.Email.Equals(_email);
}
