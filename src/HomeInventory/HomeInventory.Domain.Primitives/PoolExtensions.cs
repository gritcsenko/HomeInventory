namespace HomeInventory.Domain.Primitives;

public static class PoolExtensions
{
    public static PoolHandle<T> PullScoped<T>(this IPool<T> pool)
        where T : class
    {
        var obj = pool?.Pull() ?? throw new ArgumentNullException(nameof(pool));
        return new(pool, obj);
    }
}
