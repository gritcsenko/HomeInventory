using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Interfaces.Persistence;

public interface IUserRepository : IRepository<User, UserId>
{
}
