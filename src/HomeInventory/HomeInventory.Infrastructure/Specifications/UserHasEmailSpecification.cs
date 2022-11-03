using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class UserHasEmailSpecification : FilterSpecification<UserModel>
{
    public UserHasEmailSpecification(Email email)
        : base(x => x.Email == email.Value)
    {
    }
}
