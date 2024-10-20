using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.UserManagement.ValueObjects;

public sealed class UserId(Ulid value) : UlidIdentifierObject<UserId>(value), IUlidBuildable<UserId>
{
    public static UserId CreateFrom(Ulid value) => new(value);
}
