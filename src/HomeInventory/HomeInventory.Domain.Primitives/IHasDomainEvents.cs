namespace HomeInventory.Domain.Primitives;

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}
