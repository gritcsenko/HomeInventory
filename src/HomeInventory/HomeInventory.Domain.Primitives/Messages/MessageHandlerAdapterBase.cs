using System.Reactive.Linq;

namespace HomeInventory.Domain.Primitives.Messages;

public abstract class MessageHandlerAdapterBase<TMessage, TResponse>() : IMessageHandlerAdapter
    where TMessage : IMessage
{
    public IDisposable Subscribe(IMessageHub hub) =>
        InternalModify(hub, hub.GetMessages<TMessage>()).Subscribe();

    protected virtual IObservable<TResponse> InternalModify(IMessageHub hub, IObservable<TMessage> messages) =>
        messages.SelectMany(e => HandleMessage(hub, e));

    protected abstract IObservable<TResponse> HandleMessage(IMessageHub hub, TMessage message);
}
