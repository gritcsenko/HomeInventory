namespace HomeInventory.Domain.Primitives;

public interface IEvent
{
    Cuid Id { get; }
}
