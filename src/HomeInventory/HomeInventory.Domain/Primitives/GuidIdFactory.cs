using FluentResults;
using OneOf;

namespace HomeInventory.Domain.Primitives;

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

internal sealed class IdFactory<TId, TValue> : ValueObjectFactory<TId>, IIdFactory<TId, TValue>
    where TId : IIdentifierObject<TId>
{
    private readonly Func<TValue, TId> _createIdFunc;
    private readonly Func<TValue, bool> _isValidFunc;
    private readonly Func<TValue> _generateNewValueFunc;

    public IdFactory(Func<TValue, bool> isValidFunc, Func<TValue, TId> createIdFunc, Func<TValue> generateNewValueFunc)
    {
        _createIdFunc = createIdFunc;
        _isValidFunc = isValidFunc;
        _generateNewValueFunc = generateNewValueFunc;
    }

    public OneOf<TId, IError> CreateFrom(TValue id) => TryCreate(id, _isValidFunc, _createIdFunc);

    public TId CreateNew() => _createIdFunc(_generateNewValueFunc());
}
