using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests;

internal class FixedTestingDateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow { get; set; }
}