using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public static class GuidIdFactory
{
    public static IIdFactory<TId, Guid> Create<TId>(Func<Guid, TId> createIdFunc)
        where TId : IIdentifierObject<TId>
    {
        return new IdFactory<TId, Guid>(id => id != Guid.Empty, createIdFunc, Guid.NewGuid);
    }
}
