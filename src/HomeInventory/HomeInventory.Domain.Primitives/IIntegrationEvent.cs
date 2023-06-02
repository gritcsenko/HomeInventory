namespace HomeInventory.Domain.Primitives;

public interface IIntegrationEvent : IEvent, IHaveCreationAudit
{
    IAggregateRoot Source { get; }
}
