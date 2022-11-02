using FluentResults;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class IdFactory<TId, TValue> : ValueObjectFactory<TId>, IIdFactory<TId, TValue>
    where TId : IIdentifierObject<TId>
{
    private readonly Func<TValue, TId> _createIdFunc;
    private readonly Func<TValue, bool> _isValidFunc;
    private readonly Func<TValue> _generateNewValueFunc;

    public IdFactory(Func<TValue, TId> createIdFunc, Func<TValue, bool> isValidFunc, Func<TValue> generateNewValueFunc)
    {
        _createIdFunc = createIdFunc;
        _isValidFunc = isValidFunc;
        _generateNewValueFunc = generateNewValueFunc;
    }

    public IResult<TId> CreateFrom(TValue id) => Result.FailIf(!_isValidFunc(id), GetValidationError(id)).Bind(() => Result.Ok(_createIdFunc(id)));

    public TId CreateNew() => _createIdFunc(_generateNewValueFunc());
}
