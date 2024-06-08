namespace HomeInventory.Domain.Primitives.Messages;

public sealed record class CancellableRequest<TMessage>(TMessage Message, CancellationToken CancellationToken) : IMessage
    where TMessage : IMessage
{
    public Cuid Id => Message.Id;

    public DateTimeOffset CreatedOn => Message.CreatedOn;
}

public sealed record class CancellableRequest<TRequest, TResponse>(TRequest Message, CancellationToken CancellationToken) : IRequestMessage<TResponse>
    where TRequest : IRequestMessage<TResponse>
{
    public Cuid Id => Message.Id;

    public DateTimeOffset CreatedOn => Message.CreatedOn;
}
