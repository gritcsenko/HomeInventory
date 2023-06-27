using System.Runtime.Versioning;

namespace HomeInventory.Core;

public static class OptionalExtensions
{
    [RequiresPreviewFeatures]
    public static async Task<Optional<T>> Tap<T>(this Task<Optional<T>> optionalTask, Action<T> action)
    {
        var optional = await optionalTask;
        return optional.Tap(action);
    }

    [RequiresPreviewFeatures]
    public static async Task<Optional<T>> Tap<T>(this Task<Optional<T>> optionalTask, Func<T, Task> asyncAction)
    {
        var optional = await optionalTask;
        return await optional.Tap(asyncAction);
    }

    [RequiresPreviewFeatures]
    public static Optional<T> Tap<T>(this Optional<T> optional, Action<T> action)
    {
        if (optional.TryGet(out var value, out var isNull) && !isNull)
        {
            action(value);
        }

        return optional;
    }

    [RequiresPreviewFeatures]
    public static async Task<Optional<T>> Tap<T>(this Optional<T> optional, Func<T, Task> asyncAction)
    {
        if (optional.TryGet(out var value, out var isNull) && !isNull)
        {
            await asyncAction(value);
        }

        return optional;
    }
}
