using HomeInventory.Domain.Entities;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<bool> IsUserHasEmailAsync(string email, CancellationToken cancellationToken = default); // TODO replace with Email ValueObject

    Task<OneOf<User, NotFound>> FindFirstByEmailOrNotFoundUserAsync(string email, CancellationToken cancellationToken = default); // TODO replace with Email ValueObject
}
