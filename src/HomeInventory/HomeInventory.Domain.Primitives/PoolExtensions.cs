namespace HomeInventory.Domain.Primitives;

public static class PoolExtensions
{
    public static PoolHandle<T> PullScoped<T>(this IPool<T> pool)
        where T : class
    {
        if (pool is null)
        {
            throw new ArgumentNullException(nameof(pool));
        }

        var obj = pool.Pull();
        return new(pool, obj);
    }
}
