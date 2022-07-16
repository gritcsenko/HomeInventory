using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using MapsterMapper;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;
internal class UserRepository : IUserRepository
{
    private static readonly ICollection<User> _users = new List<User>();
    private readonly IMapper _mapper;

    public UserRepository(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<OneOf<Success>> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;
        _users.Add(entity);
        return new Success();
    }

    public async Task<OneOf<User, NotFound>> FindByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;
        return _users.FirstOrDefault(u => u.Id == id) ?? (OneOf<User, NotFound>)new NotFound();
    }

    public async Task<bool> HasEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;
        return _users.Any(u => u.Email == email);
    }

    public async Task<OneOf<User, NotFound>> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        await ValueTask.CompletedTask;
        return _users.FirstOrDefault(u => u.Email == email) ?? (OneOf<User, NotFound>)new NotFound();
    }
}
