using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Domain.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<bool> IsUserHasEmailAsync(Email email, CancellationToken cancellationToken = default);

    Task<OneOf<User, NotFound>> FindFirstByEmailOrNotFoundUserAsync(Email email, CancellationToken cancellationToken = default);

    Task<bool> HasPermissionAsync(UserId userId, string permission, CancellationToken cancellationToken = default);
}
