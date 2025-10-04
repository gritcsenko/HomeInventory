namespace HomeInventory.Domain.Primitives;

public interface IEvent
{
    Ulid Id { get; }
}
