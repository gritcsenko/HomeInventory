namespace HomeInventory.Core;

public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T>? source)
    {
        public IEnumerable<T> EmptyIfNull() => source ?? [];
    }

    extension<T>(IEnumerable<T> source)
    {
        public void ForEach(Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public IEnumerable<T> Concat(params IReadOnlyCollection<T> items) => source.Concat(items.AsEnumerable());

        public IReadOnlyCollection<T> AsReadOnly() => source as IReadOnlyCollection<T> ?? [.. source];

        public async ValueTask<bool> AllAsync(Func<T, ValueTask<bool>> predicate)
        {
            foreach (var item in source)
            {
                if (!await predicate(item))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
