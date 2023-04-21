namespace HomeInventory.Domain.Primitives;

public interface IDateTimeService
{
    DateTimeOffset Now { get; }
}
