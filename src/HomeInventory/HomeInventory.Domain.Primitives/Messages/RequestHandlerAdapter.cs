using HomeInventory.Domain.Primitives.Errors;
using System.Reactive.Linq;

namespace HomeInventory.Domain.Primitives.Messages;

public sealed class RequestHandlerAdapter<TRequest, TResult>(IRequestHandler<TRequest, TResult> requestHandler, IEnumerable<IRequestPipelineBehavior<TRequest, TResult>> behaviors) : MessageHandlerAdapterBase<CancellableRequest<TRequest>, ResposeMessage<TRequest, TResult>>
    where TRequest : IRequestMessage<TResult>
{
    private readonly IRequestHandler<TRequest, TResult> _requestHandler = requestHandler;
    private readonly IEnumerable<IRequestPipelineBehavior<TRequest, TResult>> _behaviors = behaviors.ToArray();

    protected override IDisposable Subscribe(IMessageHub hub, IObservable<ResposeMessage<TRequest, TResult>> responses) =>
        base.Subscribe(hub, responses.Do(r => hub.OnNext(r)));

    protected sealed override IObservable<ResposeMessage<TRequest, TResult>> HandleMessage(IMessageHub hub, CancellableRequest<TRequest> message) =>
        Observable.FromAsync(async () =>
        {
            var request = message.Message;
            var token = message.CancellationToken;
            var pipeline = _behaviors.Aggregate(Seed, (c, b) => () => b.OnRequest(hub, request, c, token));
            var result = await pipeline();
            return hub.CreateResponse(request, result);

            Task<OneOf<TResult, IError>> Seed() => _requestHandler.HandleAsync(hub, request, token);
        });
}
