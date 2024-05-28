
namespace HomeInventory.Domain.Primitives.Events;

public sealed record class CancellableRequestEvent<TPayload>(TPayload Payload, CancellationToken CancellationToken) : IEvent
    where TPayload : IEvent
{ 
    public Cuid Id => Payload.Id;

    public DateTimeOffset CreatedOn => Payload.CreatedOn;
}
