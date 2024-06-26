using HomeInventory.Domain.Primitives.Errors;
using System.Reactive.Linq;

namespace HomeInventory.Domain.Primitives.Messages;

public sealed class RequestHandlerAdapter<TRequest, TResult>(IRequestHandler<TRequest, TResult> requestHandler, IEnumerable<IRequestPipelineBehavior<TRequest, TResult>> behaviors) : MessageHandlerAdapterBase<CancellableRequest<TRequest, TResult>, ResposeMessage<TRequest, TResult>>
    where TRequest : IRequestMessage<TResult>
{
    private readonly IRequestHandler<TRequest, TResult> _requestHandler = requestHandler;
    private readonly IEnumerable<IRequestPipelineBehavior<TRequest, TResult>> _behaviors = behaviors.ToArray();

    protected override IDisposable Subscribe(IObservable<(IMessageHub Hub, ResposeMessage<TRequest, TResult> Message)> responses) =>
        base.Subscribe(responses.Do(x => x.Hub.OnNext(x.Message)));

    protected sealed override IObservable<ResposeMessage<TRequest, TResult>> HandleMessage(IMessageHub hub, CancellableRequest<TRequest, TResult> message) =>
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
