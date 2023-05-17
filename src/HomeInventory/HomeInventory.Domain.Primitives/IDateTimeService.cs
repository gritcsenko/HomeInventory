namespace HomeInventory.Domain.Primitives;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}
