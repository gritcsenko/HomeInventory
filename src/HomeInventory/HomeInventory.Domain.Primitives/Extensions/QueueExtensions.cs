namespace HomeInventory.Domain;

public static class QueueExtensions
{
    public static T DequeueOrDefault<T>(this Queue<T> queue, Func<T> getDefautFunc) => queue.TryDequeue(out var value) ? value : getDefautFunc();
}
