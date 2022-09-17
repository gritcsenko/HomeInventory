using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Support;
internal class FixedTestingDateTimeService : IDateTimeService
{
    public DateTimeOffset Now { get; set; }
}