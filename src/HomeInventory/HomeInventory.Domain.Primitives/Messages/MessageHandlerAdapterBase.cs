using System.Reactive.Linq;

namespace HomeInventory.Domain.Primitives.Messages;

public abstract class MessageHandlerAdapterBase<TMessage, TResponse>() : IMessageHandlerAdapter<TMessage>
    where TMessage : IMessage
{
    public IDisposable Subscribe(IObservable<(IMessageHub Hub, TMessage Message)> observable)
    {
        var responses = observable
            .SelectMany(x => HandleMessage(x.Hub, x.Message).Select(r => (x.Hub, Message: r)));
        return Subscribe(responses);
    }

    protected virtual IDisposable Subscribe(IObservable<(IMessageHub Hub, TResponse Message)> responses) => responses.Subscribe();

    protected abstract IObservable<TResponse> HandleMessage(IMessageHub hub, TMessage message);
}
