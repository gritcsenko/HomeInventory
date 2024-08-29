namespace HomeInventory.Domain.Primitives.Messages;

public interface IRequestContext<out TRequest>
    where TRequest : IMessage
{
    IMessageHub Hub { get; }

    TRequest Request { get; }

    CancellationToken RequestAborted { get; }
}
