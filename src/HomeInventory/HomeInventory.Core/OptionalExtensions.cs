namespace HomeInventory.Core;

public static class OptionalExtensions
{
    public static async ValueTask<Optional<T>> Tap<T>(this Task<Optional<T>> optionalTask, Action<T> action) =>
        await new ValueTask<Optional<T>>(optionalTask).Tap(action);

    public static async ValueTask<Optional<T>> Tap<T>(this Task<Optional<T>> optionalTask, Func<T, Task> asyncAction) =>
        await new ValueTask<Optional<T>>(optionalTask).Tap(asyncAction);

    public static async ValueTask<Optional<T>> Tap<T>(this Task<Optional<T>> optionalTask, Func<T, ValueTask> asyncAction) =>
        await new ValueTask<Optional<T>>(optionalTask).Tap(asyncAction);

    public static async ValueTask<Optional<T>> Tap<T>(this ValueTask<Optional<T>> optionalTask, Action<T> action) =>
        (await optionalTask).Tap(action);

    public static async ValueTask<Optional<T>> Tap<T>(this ValueTask<Optional<T>> optionalTask, Func<T, Task> asyncAction) =>
        await (await optionalTask).Tap(asyncAction);

    public static async ValueTask<Optional<T>> Tap<T>(this ValueTask<Optional<T>> optionalTask, Func<T, ValueTask> asyncAction) =>
        await (await optionalTask).Tap(asyncAction);

    public static Optional<T> Tap<T>(this Optional<T> optional, Action<T> action)
    {
        if (optional.HasValue)
        {
            action(optional.Value);
        }

        return optional;
    }

    public static async ValueTask<Optional<T>> Tap<T>(this Optional<T> optional, Func<T, Task> asyncAction) =>
        await optional.Tap(v => new ValueTask(asyncAction(v)));

    public static async ValueTask<Optional<T>> Tap<T>(this Optional<T> optional, Func<T, ValueTask> asyncAction)
    {
        if (optional.HasValue)
        {
            await asyncAction(optional.Value);
        }

        return optional;
    }

    public static async ValueTask<Optional<T>> IfAsync<T>(this ValueTask<Optional<T>> optionalTask, Func<T, CancellationToken, ValueTask<bool>> asyncCondition, CancellationToken cancellationToken = default)
    {
        var optional = await optionalTask;
        return optional.HasValue && await asyncCondition(optional.Value, cancellationToken)
            ? optional
            : Optional<T>.None;
    }

    public static async ValueTask<Optional<TResult>> ConvertAsync<T, TResult>(this ValueTask<Optional<T>> optionalTask, Func<T, CancellationToken, ValueTask<TResult>> asyncConverter, CancellationToken cancellationToken = default)
    {
        var optional = await optionalTask;
        return await optional.ConvertAsync(asyncConverter, cancellationToken);
    }

    public static async ValueTask<Optional<TResult>> ConvertAsync<T, TResult>(this Optional<T> optional, Func<T, CancellationToken, ValueTask<TResult>> asyncConverter, CancellationToken cancellationToken = default) =>
        optional.HasValue
            ? (Optional<TResult>)await asyncConverter(optional.Value, cancellationToken)
            : Optional<TResult>.None;

    public static async ValueTask<Optional<TResult>> Convert<T, TResult>(this ValueTask<Optional<T>> optionalTask, Converter<T, TResult> converter)
    {
        var optional = await optionalTask;
        return optional.Convert(converter);
    }

    public static Optional<T> Coalesce<T>(this Optional<T> first, Func<Optional<T>> secondFunc) => first.HasValue ? first : secondFunc();

    public static Optional<T> NoneIfNull<T>(this T? value) where T : class => value is null ? Optional<T>.None : Optional.Some(value);
}
