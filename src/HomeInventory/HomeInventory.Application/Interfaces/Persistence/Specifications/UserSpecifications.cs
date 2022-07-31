using HomeInventory.Domain.Entities;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public static class UserSpecifications
{
    public static FilterSpecification<User> HasEmail(string email) => new UserHasEmailSpecification(email);
}
