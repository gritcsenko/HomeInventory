using FluentResults;
using OneOf;

namespace HomeInventory.Domain.Primitives;

public interface IIdFactory<out TId>
    where TId : IIdentifierObject<TId>
{
    TId CreateNew();
}

public interface IIdFactory<TId, TValue> : IIdFactory<TId>
    where TId : IIdentifierObject<TId>
{
    OneOf<TId, IError> CreateFrom(TValue id);
}
