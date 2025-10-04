namespace HomeInventory.Core;

public static class ValidationExtensions
{
    public static async Task<Validation<TFailure, TResult>> MapAsync<TFailure, TSuccess, TResult>(this Task<Validation<TFailure, TSuccess>> validationTask, Func<TSuccess, Task<TResult>> f)
        where TFailure : LanguageExt.Traits.Monoid<TFailure>
    {
        var validation = await validationTask;
        return await validation.MapAsync(f);
    }

    public static async Task<Validation<TFailure, TResult>> MapAsync<TFailure, TSuccess, TResult>(this Validation<TFailure, TSuccess> validation, Func<TSuccess, Task<TResult>> f)
        where TFailure : LanguageExt.Traits.Monoid<TFailure>
        =>
        await validation.MapAsync(async s => Validation<TFailure, TResult>.Success(await f(s)));

    public static async Task<Validation<TFailure, TResult>> MapAsync<TFailure, TSuccess, TResult>(this Validation<TFailure, TSuccess> validation, Func<TSuccess, Task<Validation<TFailure, TResult>>> f)
        where TFailure : LanguageExt.Traits.Monoid<TFailure>
        =>
            validation.IsSuccess
                ? await f((TSuccess)validation)
                : Validation<TFailure, TResult>.Fail(validation.FailSpan()[0]);

    public static async Task<TMatch> MatchAsync<TSuccess, TMatch>(this Validation<Error, TSuccess> validation, Func<TSuccess, Task<TMatch>> onValue, Func<Error, TMatch> onFailure) =>
        validation.IsSuccess
            ? await onValue(validation.SuccessSpan()[0])
            : onFailure(validation.FailSpan()[0]);

    public static TResult MatchOrThrow<TSuccess, TResult>(this Validation<Error, TSuccess> validation, Func<TSuccess, TResult> convert) =>
        validation.Match(convert, seq => Error.Many(seq).Throw().Return(ShouldNotBeCalled<TResult>));

    private static TResult ShouldNotBeCalled<TResult>() => throw new ExceptionalException(nameof(ShouldNotBeCalled), -1_000_000_000);
}
