using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

internal static class QueryableExtensions
{
    public static IQueryable<T> AggregateFrom<T, TItem>(this IQueryable<T> queryable, IEnumerable<TItem> items, Func<IQueryable<T>, TItem, IQueryable<T>> aggregateFunc)
        => items.Aggregate(queryable, aggregateFunc);

    public static IQueryable<T> SplitIf<T>(this IQueryable<T> queryable, bool condition)
        where T : class =>
        condition ? queryable.AsSplitQuery() : queryable;
}
