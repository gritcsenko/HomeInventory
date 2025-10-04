namespace HomeInventory.Core;

public interface ISpannableCollection<T> : IReadOnlyCollection<T>
{
    Span<T> AsSpan();
}
