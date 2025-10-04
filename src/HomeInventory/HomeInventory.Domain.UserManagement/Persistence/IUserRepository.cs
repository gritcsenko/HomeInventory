using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Domain.UserManagement.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<bool> IsUserHasEmailAsync(Email email, CancellationToken cancellationToken = default);

    Task<Option<User>> FindFirstByEmailUserOptionalAsync(Email email, CancellationToken cancellationToken = default);

    Task<bool> HasPermissionAsync(UserId userId, string permission, CancellationToken cancellationToken = default);
}
