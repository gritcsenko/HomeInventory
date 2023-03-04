using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public class GuidIdConverter<TId> : ValueObjectConverter<TId, Guid>
    where TId : IIdentifierObject<TId>
{
    public GuidIdConverter(IIdFactory<TId, Guid> factory)
        : base(factory)
    {
    }
}
