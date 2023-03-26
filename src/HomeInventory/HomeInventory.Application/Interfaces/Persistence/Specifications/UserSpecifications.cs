using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public static class UserSpecifications
{
    public static FilterSpecification<User> HasEmail(Email email) => new UserHasEmailSpecification(email);
    public static FilterSpecification<User> HasId(UserId id) => new HasIdSpecification<User, UserId>(id);
}
