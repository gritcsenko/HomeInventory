namespace HomeInventory.Tests;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset DropSubSeconds(this DateTimeOffset source) =>
        new(source.Year, source.Month, source.Day, source.Hour, source.Minute, source.Second, source.Offset);
}
