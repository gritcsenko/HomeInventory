using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public class GuidIdConverter<TId> : ObjectConverter<GuidIdentifierObject<TId>.Builder, TId, Guid>
    where TId : notnull, GuidIdentifierObject<TId>, IBuildable<TId, GuidIdentifierObject<TId>.Builder>
{
    public GuidIdConverter()
        : base(id => id != Guid.Empty)
    {
    }
}
