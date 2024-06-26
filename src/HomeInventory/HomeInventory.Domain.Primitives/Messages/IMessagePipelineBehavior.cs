namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessagePipelineBehavior<in TMessage>
    where TMessage : IMessage
{
    Task OnRequest(IMessageHub hub, TMessage message, Func<Task> handler, CancellationToken cancellationToken = default);
}
