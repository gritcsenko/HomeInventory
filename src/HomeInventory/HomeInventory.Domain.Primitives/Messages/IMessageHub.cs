using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessageHub : IDisposable
{
    IIdSupplier<Ulid> EventIdSupplier { get; }

    TimeProvider EventCreatedTimeProvider { get; }

    void OnNext<TMessage>(TMessage message) where TMessage : IMessage;

    IObservable<TMessage> GetMessages<TMessage>() where TMessage : IMessage;
}
