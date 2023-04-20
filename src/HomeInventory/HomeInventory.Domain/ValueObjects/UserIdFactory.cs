using FluentResults;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class UserIdFactory : ValueObjectFactory<UserId>, IUserIdFactory
{
    public Result<UserId> Create(Guid id) => id == Guid.Empty ? GetValidationError(id) : CreateCore(id);

    public UserId CreateNew() => CreateCore(Guid.NewGuid());

    private UserId CreateCore(Guid id) => new(id);
}
