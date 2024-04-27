using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain;

internal sealed class SystemDateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
