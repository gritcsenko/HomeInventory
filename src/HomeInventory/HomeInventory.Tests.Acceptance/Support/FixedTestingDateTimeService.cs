using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Acceptance.Support;

internal class FixedTestingDateTimeService : IDateTimeService
{
    public DateTimeOffset Now { get; set; }
}
