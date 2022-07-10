using HomeInventory.Domain.Entities;

namespace HomeInventory.Application.Interfaces.Persistence;
public interface IUserRepository
{
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
}
