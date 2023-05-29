using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public class GuidIdConverter<TId> : BuilderObjectConverter<GuidIdentifierObjectBuilder<TId>, TId, Guid>
    where TId : class, IGuidIdentifierObject<TId>
{
    public GuidIdConverter()
        : base(GuidIdentifierObjectBuilder<TId>.IsValid)
    {
    }
}
