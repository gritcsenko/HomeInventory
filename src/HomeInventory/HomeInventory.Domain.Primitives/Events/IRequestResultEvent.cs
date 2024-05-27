using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Primitives.Events;

public interface IRequestResultEvent<out TRequest, TResult> : IEvent
{
    TRequest Request { get; }

    OneOf<TResult, IError> Result { get; }
}
