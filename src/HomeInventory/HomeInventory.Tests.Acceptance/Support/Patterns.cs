namespace HomeInventory.Tests.Acceptance.Support;
internal static class Patterns
{
    public const string DateOnly = @"(\d{2}/\d{2}/\d{4})";
    public const string CountWithDecimals = @"(\d+(?:\.\d+)?)";
    public const string QuotedName = @"""([^""]*)""";
    public const string Price = @"\$(\d+\.\d{2})";
}
