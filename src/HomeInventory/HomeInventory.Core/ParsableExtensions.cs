namespace HomeInventory.Core;

public static class ParsableExtensions
{
    extension<TSelf>(IParsable<TSelf>)
        where TSelf : IParsable<TSelf>
    {
        public static TSelf Parse(string text) =>
            TSelf.TryParse(text)
                .ThrowIfNone(() => new InvalidOperationException($"Failed to parse '{text}' to {typeof(TSelf).Name}"));
    }
}
