using DotNext;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Persistence;

public interface IUserRepository : IRepository<User>
{
    ValueTask<bool> IsUserHasEmailAsync(Email email, CancellationToken cancellationToken = default);

    ValueTask<Optional<User>> FindFirstByEmailUserOptionalAsync(Email email, CancellationToken cancellationToken = default);

    ValueTask<bool> HasPermissionAsync(UserId userId, string permission, CancellationToken cancellationToken = default);
}
