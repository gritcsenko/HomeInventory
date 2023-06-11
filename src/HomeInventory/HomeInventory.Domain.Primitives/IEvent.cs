namespace HomeInventory.Domain.Primitives;

public interface IEvent
{
    Guid Id { get; }
}
