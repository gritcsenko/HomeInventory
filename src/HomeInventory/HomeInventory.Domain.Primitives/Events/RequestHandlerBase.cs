using HomeInventory.Domain.Primitives.Errors;
using OneOf.Types;
using System.Reactive.Linq;

namespace HomeInventory.Domain.Primitives.Events;

public abstract class RequestHandlerBase<TRequest, TResponse, TResult>() : EventHandlerBase<CancellableRequestEvent<TRequest>, TResponse>
    where TRequest : IEvent
    where TResponse : IRequestResultEvent<TRequest, TResult>
{
    protected sealed override IObservable<TResponse> InternalModify(IEventHub hub, IObservable<CancellableRequestEvent<TRequest>> events)
    {
        var responses = base.InternalModify(hub, events);
        hub.Inject(responses);
        return responses;
    }

    protected sealed override IObservable<TResponse> HandleEvent(IEventHub hub, CancellableRequestEvent<TRequest> @event) =>
        Observable.FromAsync(() => HandleRequestAsync(hub, @event.Payload, @event.CancellationToken))
            .Select(result => CreateResponse(hub.EventIdSupplier.Invoke(), hub.EventCreatedTimeProvider.GetUtcNow(), @event.Payload, result));

    protected abstract Task<OneOf<TResult, IError>> HandleRequestAsync(IEventHub hub, TRequest request, CancellationToken cancellationToken);

    protected abstract TResponse CreateResponse(Cuid id, DateTimeOffset createdOn, TRequest payload, OneOf<TResult, IError> result);
}

public abstract class RequestHandlerBase<TRequest>() : RequestHandlerBase<TRequest, ResposeEvent<TRequest>, Success>()
    where TRequest : IRequestEvent
{
    protected sealed override ResposeEvent<TRequest> CreateResponse(Cuid id, DateTimeOffset createdOn, TRequest payload, OneOf<Success, IError> result) =>
        new(id, createdOn, payload, result);
}

public abstract class RequestHandlerBase<TRequest, TResult>() : RequestHandlerBase<TRequest, ResposeEvent<TRequest, TResult>, TResult>()
    where TRequest : IRequestEvent<TResult>
{
    protected sealed override ResposeEvent<TRequest, TResult> CreateResponse(Cuid id, DateTimeOffset createdOn, TRequest payload, OneOf<TResult, IError> result) =>
        new(id, createdOn, payload, result);
}
