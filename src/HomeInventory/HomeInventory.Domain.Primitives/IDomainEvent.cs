namespace HomeInventory.Domain.Primitives;

public interface IDomainEvent : IEvent, IHasCreationAudit
{
}
