using System.Reactive.Linq;
using Unit = System.Reactive.Unit;

namespace HomeInventory.Domain.Primitives.Messages;

public sealed class MessageHandlerAdapter<TMessage>(IMessageHandler<TMessage> messageHandler, IEnumerable<IMessagePipelineBehavior<TMessage>> behaviors) : MessageHandlerAdapterBase<TMessage, Unit>
    where TMessage : IMessage
{
    private readonly IMessageHandler<TMessage> _messageHandler = messageHandler;
    private readonly IEnumerable<IMessagePipelineBehavior<TMessage>> _behaviors = behaviors.ToArray();

    protected override IObservable<Unit> HandleMessage(IMessageHub hub, TMessage message) =>
        Observable.FromAsync(async ct =>
        {
            var pipeline = _behaviors.Aggregate(Seed, (c, b) => () => b.OnRequest(hub, message, c, ct));
            await pipeline();

            Task Seed() => _messageHandler.HandleAsync(hub, message, ct);
        });
}
