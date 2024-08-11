using System.Reactive.Linq;

namespace HomeInventory.Domain.Primitives.Messages;

public sealed class RequestHandlerAdapter<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> requestHandler, IEnumerable<IRequestPipelineBehavior<TRequest, TResponse>> behaviors) : MessageHandlerAdapterBase<CancellableRequest<TRequest, TResponse>, ResposeMessage<TRequest, TResponse>>
    where TRequest : IRequestMessage<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _requestHandler = requestHandler;
    private readonly IEnumerable<IRequestPipelineBehavior<TRequest, TResponse>> _behaviors = behaviors.ToArray();

    protected override IDisposable Subscribe(IObservable<(IMessageHub Hub, ResposeMessage<TRequest, TResponse> Message)> responses) =>
        base.Subscribe(responses.Do(x => x.Hub.OnNext(x.Message)));

    protected sealed override IObservable<ResposeMessage<TRequest, TResponse>> HandleMessage(IMessageHub hub, CancellableRequest<TRequest, TResponse> message) =>
        Observable.FromAsync(async () =>
        {
            var pipeline = _behaviors.Aggregate(Seed, (handler, behavior) => ctx => behavior.OnRequestAsync(ctx, handler));
            var result = await pipeline(new RequestContext(hub, message));
            return hub.CreateResponse(message.Message, result);

            Task<TResponse> Seed(IRequestContext<TRequest> ctx) => _requestHandler.HandleAsync(ctx);
        });

    private sealed record class RequestContext(IMessageHub Hub, TRequest Request, CancellationToken RequestAborted) : IRequestContext<TRequest>
    {
        public RequestContext(IMessageHub hub, CancellableRequest<TRequest, TResponse> request)
            : this(hub, request.Message, request.CancellationToken)
        {
        }
    }
}
