using ErrorOr;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

internal class UserIdFactory : ValueObjectFactory<UserId>, IUserIdFactory
{
    public ErrorOr<UserId> Create(Guid id) => id == Guid.Empty ? GetValidationError() : CreateCore(id);

    public UserId CreateNew() => CreateCore(Guid.NewGuid());

    private UserId CreateCore(Guid id) => new(id);
}
