namespace HomeInventory.Core;

public static class CollectionExtensions
{
    extension<T>(ICollection<T> collection)
    {
        public void AddRange(IEnumerable<T> items)
        {
            switch (collection)
            {
                case List<T> list:
                    list.AddRange(items);
                    break;
                default:
                    items.ForEach(collection.Add);
                    break;
            }
        }
    }

    public static Optional<T> PeekOrNone<T>(this Stack<T> stack)
    {
        if (stack.TryPeek(out var item))
        {
            return item;
        }

        return Optional<T>.None;
    }
}
