namespace HomeInventory.Core;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
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
