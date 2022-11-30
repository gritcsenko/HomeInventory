using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public static class GuidIdFactory
{
    public static IIdFactory<TId, Guid> Create<TId>(Func<Guid, TId> createIdFunc)
        where TId : IIdentifierObject<TId>
    {
        return new IdFactory<TId, Guid>(id => id != Guid.Empty, createIdFunc, Guid.NewGuid);
    }

    public static IIdFactory<TId, string> CreateFromString<TId>(Func<Guid, TId> createIdFunc)
        where TId : IIdentifierObject<TId>
    {
        return new IdFactory<TId, string>(text => Guid.TryParse(text, out var id) && id != Guid.Empty, text => createIdFunc(Guid.Parse(text)), () => Guid.NewGuid().ToString());
    }
}
