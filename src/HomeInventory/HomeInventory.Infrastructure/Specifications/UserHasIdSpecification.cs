using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class UserHasIdSpecification : FilterSpecification<UserModel>
{
    public UserHasIdSpecification(UserId id)
        : base(x => x.Id == id.Id)
    {
    }
}
