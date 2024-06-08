using System.Reactive.Linq;

namespace HomeInventory.Domain.Primitives.Messages;

public abstract class MessageHandlerAdapterBase<TMessage, TResponse>() : IMessageHandlerAdapter
    where TMessage : IMessage
{
    public IDisposable Subscribe(IMessageHub hub)
    {
        var responses = hub
            .GetMessages<TMessage>()
            .SelectMany(e => HandleMessage(hub, e));
        return Subscribe(hub, responses);
    }

    protected virtual IDisposable Subscribe(IMessageHub hub, IObservable<TResponse> responses) => responses.Subscribe();

    protected abstract IObservable<TResponse> HandleMessage(IMessageHub hub, TMessage message);
}
