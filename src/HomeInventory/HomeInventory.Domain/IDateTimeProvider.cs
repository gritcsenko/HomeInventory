namespace HomeInventory.Domain;

public interface IDateTimeService
{
    DateTimeOffset Now { get; }
}
