using FluentResults;

namespace HomeInventory.Domain.Primitives;

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
