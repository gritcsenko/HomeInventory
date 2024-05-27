using HomeInventory.Domain.Primitives.Errors;
using OneOf.Types;

namespace HomeInventory.Domain.Primitives.Events;

public abstract class RequestHandler<TRequest>(ISupplier<Cuid> supplier) : EventHandlerBase<TRequest>
    where TRequest : IRequestEvent
{
    private readonly ISupplier<Cuid> _supplier = supplier;

    protected sealed override async Task HandleEventAsync(IEventHub hub, TRequest @event, CancellationToken cancellationToken)
    {
        var result = await HandleRequestAsync(hub, @event, cancellationToken);
        var response = new ResposeEvent
        {
            Id = _supplier.Invoke(),
            Request = @event,
            Result = result,
        };
        hub.Notify<IResposeEvent<TRequest>>(response);
    }

    protected abstract Task<OneOf<Success, IError>> HandleRequestAsync(IEventHub hub, TRequest request, CancellationToken cancellationToken);

    private sealed class ResposeEvent : IResposeEvent<TRequest>
    {
        public required TRequest Request { get; init; }
        public required OneOf<Success, IError> Result { get; init; }
        public required Cuid Id { get; init; }
    }
}

public abstract class RequestHandler<TRequest, TResult>(ISupplier<Cuid> supplier) : EventHandlerBase<TRequest>
    where TRequest : IRequestEvent<TResult>
{
    private readonly ISupplier<Cuid> _supplier = supplier;

    protected sealed override async Task HandleEventAsync(IEventHub hub, TRequest @event, CancellationToken cancellationToken)
    {
        var result = await HandleRequestAsync(hub, @event, cancellationToken);
        var response = new ResposeEvent
        {
            Id = _supplier.Invoke(),
            Request = @event,
            Result = result,
        };
        hub.Notify<IResposeEvent<TRequest, TResult>>(response);
    }

    protected abstract Task<OneOf<TResult, IError>> HandleRequestAsync(IEventHub hub, TRequest request, CancellationToken cancellationToken);

    private sealed class ResposeEvent : IResposeEvent<TRequest, TResult>
    {
        public required TRequest Request { get; init; }
        public required OneOf<TResult, IError> Result { get; init; }
        public required Cuid Id { get; init; }
    }
}

