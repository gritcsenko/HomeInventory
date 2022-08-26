using OneOf;
using OneOf.Types;

namespace HomeInventory.Domain.Events;

public interface IEvent
{
    DateTimeOffset TimeStamp { get; }

    OneOf<object, None> Source { get; }
}

public interface IEvent<TSource> : IEvent
{
    OneOf<TSource, None> TypedSource { get; }
}


