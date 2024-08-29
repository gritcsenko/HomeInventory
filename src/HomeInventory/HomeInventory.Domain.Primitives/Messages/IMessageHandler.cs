namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessageHandler<in TMessage>
    where TMessage : IMessage
{
    Task HandleAsync(IMessageHub hub, TMessage message, CancellationToken cancellationToken = default);
}
