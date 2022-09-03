namespace HomeInventory.Tests.Acceptance.Support;
internal static class StringExtensions
{
    public static DateOnly ParseDate(this string text, string format = "MM/dd/yyyy") => DateOnly.ParseExact(text, format);
    public static decimal ParseDecimal(this string text) => decimal.Parse(text);
}
