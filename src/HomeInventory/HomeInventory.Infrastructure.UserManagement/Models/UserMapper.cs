using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Infrastructure.Framework;
using Riok.Mapperly.Abstractions;

namespace HomeInventory.Infrastructure.UserManagement.Models;

[Mapper]
internal partial class UserMapper(TimeProvider timeProvider) : IPersistentMapper<UserModel, User, UserId>
{
    private readonly TimeProvider _timeProvider = timeProvider;

    public UserModel ToPersistent(User entity)
    {
        var now = _timeProvider.GetUtcNow();
        return new()
        {
            Id = entity.Id,
            Email = entity.Email.Value,
            PasswordHash = entity.PasswordHash,
            CreatedOn = now,
            ModifiedOn = now,
        };
    }

    public UserModel ToPersistent(User entity, UserModel existing)
    {
        var now = _timeProvider.GetUtcNow();
        existing.Email = entity.Email.Value;
        existing.PasswordHash = entity.PasswordHash;
        existing.ModifiedOn = now;
        return existing;
    }

    public IQueryable<User> FromPersistent(IQueryable<UserModel> queryable)
        => queryable.Select(x => new User(x.Id)
        {
            Email = new(x.Email),
            PasswordHash = x.PasswordHash,
        });
}
