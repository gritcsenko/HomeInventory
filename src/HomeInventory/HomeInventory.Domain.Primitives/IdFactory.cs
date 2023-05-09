using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Domain.Primitives;

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
