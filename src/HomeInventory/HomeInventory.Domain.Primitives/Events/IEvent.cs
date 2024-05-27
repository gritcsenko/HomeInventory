namespace HomeInventory.Domain.Primitives.Events;

public interface IEvent
{
    Cuid Id { get; }
}
