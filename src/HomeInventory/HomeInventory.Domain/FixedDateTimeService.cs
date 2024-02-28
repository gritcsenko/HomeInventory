using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain;

internal sealed class FixedDateTimeService(IDateTimeService source) : IDateTimeService
{
    public DateTimeOffset UtcNow { get; } = source.UtcNow;
}
