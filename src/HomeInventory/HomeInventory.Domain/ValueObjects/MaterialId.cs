using System.Runtime.Versioning;
using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class MaterialId : GuidIdentifierObject<MaterialId>, IBuildable<MaterialId, GuidIdentifierObject<MaterialId>.Builder>
{
    internal MaterialId(Guid value)
        : base(value)
    {
    }

    [RequiresPreviewFeatures]
    public static Builder CreateBuilder() => new(id => new MaterialId(id));
}
