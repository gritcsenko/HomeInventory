namespace HomeInventory.Core;

public static class OptionExtensions
{
    public static async Task<Option<T>> Tap<T>(this ValueTask<Option<T>> optionTask, Action<T> action) =>
        await optionTask.AsTask().Tap(action);

    public static async Task<Option<T>> Tap<T>(this ValueTask<Option<T>> optionTask, Func<T, Task> asyncAction) =>
        await optionTask.AsTask().Tap(asyncAction);

    public static async Task<Option<T>> Tap<T>(this ValueTask<Option<T>> optionTask, Func<T, ValueTask> asyncAction) =>
        await optionTask.AsTask().Tap(asyncAction);

    public static async Task<Option<T>> Tap<T>(this Task<Option<T>> optionTask, Action<T> action) =>
        (await optionTask).Tap(action);

    public static async Task<Option<T>> Tap<T>(this Task<Option<T>> optionTask, Func<T, Task> asyncAction) =>
        await (await optionTask).Tap(asyncAction);

    public static async Task<Option<T>> Tap<T>(this Task<Option<T>> optionTask, Func<T, ValueTask> asyncAction) =>
        await (await optionTask).Tap(asyncAction);

    public static Option<T> Tap<T>(this Option<T> option, Action<T> action)
    {
        if (option)
        {
            action((T)option);
        }

        return option;
    }

    public static async Task<Option<T>> Tap<T>(this Option<T> option, Func<T, ValueTask> asyncAction) =>
        await option.Tap(v => asyncAction(v).AsTask());

    public static async Task<Option<T>> Tap<T>(this Option<T> option, Func<T, Task> asyncAction)
    {
        if (option)
        {
            await asyncAction((T)option);
        }

        return option;
    }

    public static async Task<Option<T>> IfAsync<T>(this Task<Option<T>> optionTask, Func<T, CancellationToken, Task<bool>> asyncCondition, CancellationToken cancellationToken = default)
    {
        var option = await optionTask;
        return option.IsSome && await asyncCondition((T)option, cancellationToken)
            ? option
            : Option<T>.None;
    }

    public static async Task<TMatch> MatchAsync<T, TMatch>(this Option<T> option, Func<T, Task<TMatch>> onValue, Func<Task<TMatch>> onNone) =>
        option.IsSome
            ? await onValue((T)option)
            : await onNone();

    public static async Task<TMatch> MatchAsync<T, TMatch>(this Option<T> option, Func<T, Task<TMatch>> onValue, Func<TMatch> onNone) =>
        option.IsSome
            ? await onValue((T)option)
            : onNone();

    public static async Task<Option<TMatch>> MatchAsync<T, TMatch>(this Option<T> option, Func<T, Task<TMatch>> onValue) =>
        option.IsSome
            ? await onValue((T)option)
            : Option<TMatch>.None;

    public static async Task<Option<TResult>> ConvertAsync<T, TResult>(this Task<Option<T>> optionTask, Func<T, CancellationToken, Task<TResult>> asyncConverter, CancellationToken cancellationToken = default)
    {
        var option = await optionTask;
        return await option.ConvertAsync(asyncConverter, cancellationToken);
    }

    public static async Task<Option<TResult>> ConvertAsync<T, TResult>(this Option<T> option, Func<T, CancellationToken, Task<TResult>> asyncConverter, CancellationToken cancellationToken = default) =>
        await option
            .MatchAsync(async x => await asyncConverter(x, cancellationToken));

    public static async Task<Option<TResult>> Convert<T, TResult>(this Task<Option<T>> optionTask, Func<T, TResult> converter)
    {
        var option = await optionTask;
        return option.Select(converter);
    }

    public static Option<T> Coalesce<T>(this Option<T> first, Func<Option<T>> secondFunc) => first ? first : secondFunc();

    public static Option<T> NoneIfNull<T>(this T? value) where T : class => value ?? Option<T>.None;

    public static T ThrowIfNone<T>(this Option<T> option, Func<Exception> exceptionFactory) =>
        option.IfNone(() => exceptionFactory().Rethrow<T>());

    public static Validation<Error, T> ErrorIfNone<T>(this Option<Validation<Error, T>> option, Func<Error> errorFactory) =>
        option.IfNone(() => errorFactory());

    public static Validation<Error, T> ErrorIfNone<T>(this Option<T> option, Func<Error> errorsFactory) =>
        option
            .Map(Validation<Error, T>.Success)
            .IfNone(() => Validation<Error, T>.Fail(errorsFactory()));

    public static Validation<Error, T> ErrorIfNone<T>(this Option<T> option, Error error) =>
        option.ErrorIfNone(() => error);
}
