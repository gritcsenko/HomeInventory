using OneOf;

namespace HomeInventory.Core;

public static class OptionalExtensions
{
    public static async Task<Optional<T>> Tap<T>(this Task<Optional<T>> optionalTask, Action<T> action)
    {
        var optional = await optionalTask;
        return optional.Tap(action);
    }

    public static async Task<Optional<T>> Tap<T>(this Task<Optional<T>> optionalTask, Func<T, Task> asyncAction)
    {
        var optional = await optionalTask;
        return await optional.Tap(asyncAction);
    }

    public static Optional<T> Tap<T>(this Optional<T> optional, Action<T> action)
    {
        if (optional.TryGet(out var value))
        {
            action(value);
        }

        return optional;
    }

    public static async Task<Optional<T>> Tap<T>(this Optional<T> optional, Func<T, Task> asyncAction)
    {
        if (optional.TryGet(out var value))
        {
            await asyncAction(value);
        }

        return optional;
    }

    public static OneOf<TResult, TError> ToResultOrError<T, TResult, TError>(this Optional<T> optional, Func<T, TResult> toResult, Func<TError> toError) =>
        optional.TryGet(out var value)
            ? toResult(value)
            : toError();

    public static async Task<Optional<TResult>> Select<T, TResult>(this Task<Optional<T>> optionalTask, Func<T, TResult> selector)
    {
        var optional = await optionalTask;
        return optional.Select(selector);
    }

    public static async Task<Optional<TResult>> Select<T, TResult>(this Task<Optional<T>> optionalTask, Func<T, Task<TResult>> selector)
    {
        var optional = await optionalTask;
        return await optional.Select(selector);
    }

    public static Optional<TResult> Select<T, TResult>(this Optional<T> optional, Func<T, TResult> selector) =>
        optional.TryGet(out var value)
            ? selector(value)
            : Optional<TResult>.None;

    public static async Task<Optional<TResult>> Select<T, TResult>(this Optional<T> optional, Func<T, Task<TResult>> selector) =>
        optional.TryGet(out var value)
            ? await selector(value)
            : Optional<TResult>.None;

    public static async Task<Optional<T>> Where<T>(this Task<Optional<T>> optionalTask, Func<T, bool> condition)
    {
        var optional = await optionalTask;
        return optional.Where(condition);
    }

    public static async Task<Optional<T>> Where<T>(this Task<Optional<T>> optionalTask, Func<T, Task<bool>> condition)
    {
        var optional = await optionalTask;
        return await optional.Where(condition);
    }

    public static Optional<T> Where<T>(this Optional<T> optional, Func<T, bool> condition) =>
        optional.TryGet(out var value) && condition(value)
            ? optional
            : Optional<T>.None;

    public static async Task<Optional<T>> Where<T>(this Optional<T> optional, Func<T, Task<bool>> condition) =>
        optional.TryGet(out var value) && await condition(value)
            ? optional
            : Optional<T>.None;
}
