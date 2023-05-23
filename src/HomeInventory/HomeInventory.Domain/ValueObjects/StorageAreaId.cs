using System.Runtime.Versioning;
using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StorageAreaId : GuidIdentifierObject<StorageAreaId>, IBuildable<StorageAreaId, GuidIdentifierObject<StorageAreaId>.Builder>
{
    internal StorageAreaId(Guid value)
        : base(value)
    {
    }

    [RequiresPreviewFeatures]
    public static Builder CreateBuilder() => new(id => new StorageAreaId(id));
}
