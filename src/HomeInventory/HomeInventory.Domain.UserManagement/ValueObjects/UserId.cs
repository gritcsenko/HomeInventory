using DotNext;
using HomeInventory.Domain.Primitives.Ids;
using Visus.Cuid;

namespace HomeInventory.Domain.ValueObjects;

public sealed class UserId : CuidIdentifierObject<UserId>, ICuidBuildable<UserId>
{
    private UserId(Cuid value)
        : base(value)
    {
    }

    public static Result<UserId> CreateFrom(Cuid value) => Result.FromValue(new UserId(value));
}
