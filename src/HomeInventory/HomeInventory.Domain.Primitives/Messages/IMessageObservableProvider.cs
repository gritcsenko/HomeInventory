namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessageObservableProvider
{
    IObservable<TMessage> GetObservable<TMessage>(IMessageHub hub) where TMessage : IMessage;
}
