namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessageHub : IDisposable
{
    ISupplier<Cuid> EventIdSupplier { get; }

    TimeProvider EventCreatedTimeProvider { get; }

    void Inject<TMessage>(IObservable<TMessage> messages) where TMessage : IMessage;

    IObservable<TMessage> GetMessages<TMessage>() where TMessage : IMessage;
}
