using HomeInventory.Application.Interfaces.Services;

namespace HomeInventory.Infrastructure.Services;
internal class SystemDateTimeService : IDateTimeService
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
