namespace HomeInventory.Core;

public static class OptionExtensions
{
    extension<T>(ValueTask<Option<T>> optionTask)
    {
        public async Task<Option<T>> Tap(Action<T> action) =>
            await optionTask.AsTask().Tap(action);

        public async Task<Option<T>> Tap(Func<T, Task> asyncAction) =>
            await optionTask.AsTask().Tap(asyncAction);

        public async Task<Option<T>> Tap(Func<T, ValueTask> asyncAction) =>
            await optionTask.AsTask().Tap(asyncAction);
    }

    extension<T>(Task<Option<T>> optionTask)
    {
        public async Task<Option<T>> Tap(Action<T> action) =>
            (await optionTask).Tap(action);

        public async Task<Option<T>> Tap(Func<T, Task> asyncAction) =>
            await (await optionTask).Tap(asyncAction);

        public async Task<Option<T>> Tap(Func<T, ValueTask> asyncAction) =>
            await (await optionTask).Tap(asyncAction);

        public async Task<Option<T>> IfAsync(Func<T, CancellationToken, Task<bool>> asyncCondition, CancellationToken cancellationToken = default)
        {
            var option = await optionTask;
            return option.IsSome && await asyncCondition((T)option, cancellationToken)
                ? option
                : Option<T>.None;
        }

        public async Task<Option<TResult>> ConvertAsync<TResult>(Func<T, CancellationToken, Task<TResult>> asyncConverter, CancellationToken cancellationToken = default)
        {
            var option = await optionTask;
            return await option.ConvertAsync(asyncConverter, cancellationToken);
        }

        public async Task<Option<TResult>> Convert<TResult>(Func<T, TResult> converter)
        {
            var option = await optionTask;
            return option.Select(converter);
        }
    }

    extension<T>(Option<T> option)
    {
        public Option<T> Tap(Action<T> action)
        {
            if (option)
            {
                action((T)option);
            }

            return option;
        }

        public async Task<Option<T>> Tap(Func<T, ValueTask> asyncAction) =>
            await option.Tap(v => asyncAction(v).AsTask());

        public async Task<Option<T>> Tap(Func<T, Task> asyncAction)
        {
            if (option)
            {
                await asyncAction((T)option);
            }

            return option;
        }

        public Option<T> Coalesce(Func<Option<T>> secondFunc) => option ? option : secondFunc();

        public T ThrowIfNone(Func<Exception> exceptionFactory) =>
            option.IfNone(() => exceptionFactory().Rethrow<T>());

        public Validation<Error, T> ErrorIfNone(Func<Error> errorsFactory) =>
            option
                .Map(Validation.Success<Error, T>)
                .IfNone(() => Validation.Fail<Error, T>(errorsFactory()));

        public Validation<Error, T> ErrorIfNone(Error error) =>
            option.ErrorIfNone(() => error);

        public async Task<Option<TResult>> ConvertAsync<TResult>(Func<T, CancellationToken, Task<TResult>> asyncConverter, CancellationToken cancellationToken = default) =>
            await option
                .MatchAsync(async x => await asyncConverter(x, cancellationToken));

        public async Task<Option<TMatch>> MatchAsync<TMatch>(Func<T, Task<TMatch>> onValue) =>
            option.IsSome
                ? await onValue((T)option)
                : Option<TMatch>.None;

    }

    public static Option<T> NoneIfNull<T>(this T? value) where T : class => value ?? Option<T>.None;

    public static Validation<Error, T> ErrorIfNone<T>(this Option<Validation<Error, T>> option, Func<Error> errorFactory) =>
        option.IfNone(() => errorFactory());
}
