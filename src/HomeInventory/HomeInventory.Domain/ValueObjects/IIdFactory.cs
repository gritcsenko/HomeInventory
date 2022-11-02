using FluentResults;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public interface IIdFactory<out TId>
    where TId : IIdentifierObject<TId>
{
    TId CreateNew();
}

public interface IIdFactory<out TId, in TValue> : IIdFactory<TId>
    where TId : IIdentifierObject<TId>
{
    IResult<TId> CreateFrom(TValue id);
}
