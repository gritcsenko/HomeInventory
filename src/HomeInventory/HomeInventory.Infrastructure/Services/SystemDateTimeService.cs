using HomeInventory.Domain;

namespace HomeInventory.Infrastructure.Services;
internal class SystemDateTimeService : IDateTimeService
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public DateOnly Today => DateOnly.FromDateTime(Now.Date);
}
