namespace HomeInventory.Tests.Acceptance.Support;

internal static class StringExtensions
{
    public static DateOnly ParseDate(this string text, string format = "MM/dd/yyyy", IFormatProvider? formatProvider = null) => DateOnly.ParseExact(text, format, formatProvider);

    public static decimal ParseDecimal(this string text, IFormatProvider? formatProvider = null) => decimal.Parse(text, formatProvider);
}
