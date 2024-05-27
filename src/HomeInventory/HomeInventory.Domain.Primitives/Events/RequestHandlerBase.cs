using HomeInventory.Domain.Primitives.Errors;
using OneOf.Types;

namespace HomeInventory.Domain.Primitives.Events;

public abstract class RequestHandlerBase<TRequest, TResponse, TResult>(ISupplier<Cuid> supplier) : EventHandlerBase<CancellableRequestEvent<TRequest>>
    where TRequest : IEvent
    where TResponse : IRequestResultEvent<TRequest, TResult>
{
    private readonly ISupplier<Cuid> _supplier = supplier;
    protected sealed override async Task HandleEventAsync(IEventHub hub, CancellableRequestEvent<TRequest> @event, CancellationToken cancellationToken)
    {
        using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, @event.CancellationToken);
        var result = await HandleRequestAsync(hub, @event.Payload, source.Token);
        var response = CreateResponse(_supplier.Invoke(), @event.Payload, result);
        hub.Notify(response);
    }

    protected abstract Task<OneOf<TResult, IError>> HandleRequestAsync(IEventHub hub, TRequest request, CancellationToken cancellationToken);

    protected abstract TResponse CreateResponse(Cuid id, TRequest payload, OneOf<TResult, IError> result);
}

public abstract class RequestHandlerBase<TRequest>(ISupplier<Cuid> supplier) : RequestHandlerBase<TRequest, IResposeEvent<TRequest>, Success>(supplier)
    where TRequest : IRequestEvent
{
    protected sealed override IResposeEvent<TRequest> CreateResponse(Cuid id, TRequest payload, OneOf<Success, IError> result) =>
        new ResposeEvent(id, payload, result);

    private sealed record class ResposeEvent(Cuid Id, TRequest Request, OneOf<Success, IError> Result) : IResposeEvent<TRequest>;
}

public abstract class RequestHandlerBase<TRequest, TResult>(ISupplier<Cuid> supplier) : RequestHandlerBase<TRequest, IResposeEvent<TRequest, TResult>, TResult>(supplier)
    where TRequest : IRequestEvent<TResult>
{
    protected sealed override IResposeEvent<TRequest, TResult> CreateResponse(Cuid id, TRequest payload, OneOf<TResult, IError> result) =>
        new ResposeEvent(id, payload, result);

    private sealed record class ResposeEvent(Cuid Id, TRequest Request, OneOf<TResult, IError> Result) : IResposeEvent<TRequest, TResult>;
}
