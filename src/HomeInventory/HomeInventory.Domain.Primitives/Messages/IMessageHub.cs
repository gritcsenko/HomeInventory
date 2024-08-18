namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessageHub : IDisposable
{
    IMessageHubContext Context { get; }

    void OnNext<TMessage>(TMessage message) where TMessage : IMessage;

    IObservable<TMessage> GetMessages<TMessage>() where TMessage : IMessage;
}
