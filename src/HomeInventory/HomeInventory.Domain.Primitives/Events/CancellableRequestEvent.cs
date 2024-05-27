namespace HomeInventory.Domain.Primitives.Events;

public sealed record class CancellableRequestEvent<TPayload>(Cuid Id, TPayload Payload, CancellationToken CancellationToken) : IEvent;

