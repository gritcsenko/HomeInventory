using System.Runtime.CompilerServices;

namespace HomeInventory.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item) => source.Concat(Enumerable.Repeat(item, 1));

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source) => source.SelectMany(Func.Identity<IEnumerable<T>>());

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? Array.Empty<T>();

    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source) => source as IReadOnlyCollection<T> ?? source.ToArray();

    public static Queue<T> ToQueue<T>(this IEnumerable<T> source) => new(source);

    public static async IAsyncEnumerable<TResult> ParallelSelectAsync<TSource, TResult>(
        this IEnumerable<TSource> items,
        Func<TSource, CancellationToken, Task<TResult>> selectAsync,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var remainingTasks = items.Select(x => selectAsync(x, cancellationTokenSource.Token)).ToHashSet();

        try
        {
            while (remainingTasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(remainingTasks);
                if (remainingTasks.Remove(completedTask))
                    yield return completedTask.GetAwaiter().GetResult();
            }
        }
        finally
        {
            cancellationTokenSource.Cancel();
            if (remainingTasks.Count > 0)
            {
                await Execute.AndCatchAsync(
                    async () => await Task.WhenAll(remainingTasks),
                    (OperationCanceledException ex) => { });
            }
        }
    }

    public static async IAsyncEnumerable<TResult> SelectAsync<TSource, TResult>(
        this IEnumerable<TSource> items,
        Func<TSource, CancellationToken, Task<TResult>> selectAsync,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var remainingTasks = items.Select(x => selectAsync(x, cancellationTokenSource.Token)).ToQueue();

        try
        {
            while (remainingTasks.Count > 0)
            {
                yield return await remainingTasks.Dequeue();
            }
        }
        finally
        {
            cancellationTokenSource.Cancel();
            if (remainingTasks.Count > 0)
            {
                await Execute.AndCatchAsync(
                    async () => await Task.WhenAll(remainingTasks),
                    (OperationCanceledException ex) => { });
            }
        }
    }
}
