using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain;

internal class SystemDateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.Now;
}
