using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class UserId : UlidIdentifierObject<UserId>, IUlidBuildable<UserId>
{
    private UserId(Ulid value)
        : base(value)
    {
    }

    public static Result<UserId> CreateFrom(Ulid value) => Result.FromValue(new UserId(value));
}
