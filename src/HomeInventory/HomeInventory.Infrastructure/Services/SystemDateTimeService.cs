using HomeInventory.Domain.Primitives;

namespace HomeInventory.Infrastructure.Services;
internal class SystemDateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
