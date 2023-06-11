namespace HomeInventory.Domain.Primitives;

public interface IIntegrationEvent : IEvent, IHaveCreationAudit
{
    IHasDomainEvents Source { get; }
}
