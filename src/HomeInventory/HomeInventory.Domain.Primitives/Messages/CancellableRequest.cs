namespace HomeInventory.Domain.Primitives.Messages;

public sealed record class CancellableRequest<TMessage>(TMessage Message, CancellationToken CancellationToken) : IMessage
    where TMessage : IMessage
{
    public Cuid Id => Message.Id;

    public DateTimeOffset CreatedOn => Message.CreatedOn;
}
