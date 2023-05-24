using Ardalis.Specification;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class UserHasEmailSpecification : Specification<UserModel>, ISingleResultSpecification<UserModel>
{
    public UserHasEmailSpecification(Email email)
    {
        Query.Where(x => x.Email == email.Value);
    }
}
