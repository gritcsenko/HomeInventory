using System.Runtime.Versioning;
using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class UserId : GuidIdentifierObject<UserId>, IBuildable<UserId, GuidIdentifierObject<UserId>.Builder>
{
    internal UserId(Guid value)
        : base(value)
    {
    }

    [RequiresPreviewFeatures]
    public static Builder CreateBuilder() => new(id => new UserId(id));
}
