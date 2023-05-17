using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain;

internal class FixedDateTimeService : IDateTimeService
{
    public FixedDateTimeService(DateTimeOffset time) => UtcNow = time.ToUniversalTime();

    public FixedDateTimeService(IDateTimeService source) => UtcNow = source.UtcNow;

    public DateTimeOffset UtcNow { get; }
}
