namespace HomeInventory.Domain.Primitives;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> GetAndClearEvents();
}
