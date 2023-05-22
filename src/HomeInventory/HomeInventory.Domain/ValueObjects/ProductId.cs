using System.Runtime.Versioning;
using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;
public sealed class ProductId : GuidIdentifierObject<ProductId>, IBuildable<ProductId, GuidIdentifierObject<ProductId>.Builder>
{
    internal ProductId(Guid value)
        : base(value)
    {
    }

    [RequiresPreviewFeatures]
    public static Builder CreateBuilder() => new(id => new ProductId(id));
}
