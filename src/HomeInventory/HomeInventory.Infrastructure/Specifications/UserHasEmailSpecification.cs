using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class UserHasEmailSpecification : FilterSpecification<UserModel>
{
    public UserHasEmailSpecification(string email)
        : base(x => x.Email == email)
    {
    }
}
