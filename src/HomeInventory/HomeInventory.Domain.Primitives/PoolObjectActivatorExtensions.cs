namespace HomeInventory.Domain.Primitives;

public static class PoolObjectActivatorExtensions
{
    public static IEnumerable<T> Pull<T>(this IPoolObjectActivator<T> activator, int count)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(activator);
        return activator.PullInternal(count);
    }

    private static IEnumerable<T> PullInternal<T>(this IPoolObjectActivator<T> activator, int count)
        where T : class
    {
        for (var i = 0; i < count; i++)
        {
            yield return activator.Pull();
        }
    }
}
