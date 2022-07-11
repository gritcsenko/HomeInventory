using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Infrastructure.Persistence;
internal class UserRepository : IUserRepository
{
    private static readonly ICollection<User> _users = new List<User>();

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Add(user);
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _users.FirstOrDefault(u => u.Email == email);
    }

    public async Task<bool> HasEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _users.Any(u => u.Email == email);
    }
}
