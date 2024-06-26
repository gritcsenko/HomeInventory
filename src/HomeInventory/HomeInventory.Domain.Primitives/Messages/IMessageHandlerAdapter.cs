namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessageHandlerAdapter<TMessage>
    where TMessage : IMessage
{
    IDisposable Subscribe(IObservable<(IMessageHub Hub, TMessage Message)> observable);
}
