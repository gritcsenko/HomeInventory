namespace HomeInventory.Domain.Primitives.Messages;

public sealed record class CancellableRequest<TRequest, TResponse>(TRequest Message, CancellationToken CancellationToken) : IRequestMessage<TResponse>
    where TRequest : IRequestMessage<TResponse>
{
    public Ulid Id => Message.Id;

    public DateTimeOffset CreatedOn => Message.CreatedOn;
}
