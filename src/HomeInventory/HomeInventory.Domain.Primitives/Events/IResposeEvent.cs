using OneOf.Types;

namespace HomeInventory.Domain.Primitives.Events;

public interface IResposeEvent<out TRequest> : IRequestResultEvent<TRequest, Success>
    where TRequest : IRequestEvent
{
}
public interface IResposeEvent<out TRequest, TResult> : IRequestResultEvent<TRequest, TResult>
    where TRequest : IRequestEvent<TResult>
{
}
