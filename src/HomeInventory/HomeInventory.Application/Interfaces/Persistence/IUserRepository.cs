using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Persistence;

public interface IUserRepository : IRepository<User, UserId>
{
    Task<OneOf<User, NotFound>> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> HasEmailAsync(string email, CancellationToken cancellationToken = default);
}
