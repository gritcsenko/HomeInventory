using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain;

internal sealed class FixedDateTimeService(DateTimeOffset time) : IDateTimeService
{
    public FixedDateTimeService(IDateTimeService source)
        : this(source.UtcNow)
    {
    }

    public DateTimeOffset UtcNow { get; } = time.ToUniversalTime();
}
