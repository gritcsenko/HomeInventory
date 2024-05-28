namespace HomeInventory.Domain.Primitives.Events;

public interface IEvent : IHasCreationAudit
{
    Cuid Id { get; }
}
