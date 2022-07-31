namespace HomeInventory.Domain;

public static class DateTimeServiceExtensions
{
    public static DateOnly Today(this IDateTimeService service) => DateOnly.FromDateTime(service.Now.Date);
}
