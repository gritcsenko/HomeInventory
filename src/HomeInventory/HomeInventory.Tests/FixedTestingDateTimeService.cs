using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests;

internal class FixedTestingDateTimeService : IDateTimeService
{
    public DateTimeOffset Now { get; set; }
}