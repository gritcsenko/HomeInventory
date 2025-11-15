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
    {
        return await validation.Match(
            Fail: fail => Task.FromResult(Validation.Fail<TFailure, TResult>(fail)),
            Succ: async succ => Validation.Success<TFailure, TResult>(await f(succ))
        );
    }

    public static async Task<Validation<TFailure, TResult>> MapAsync<TFailure, TSuccess, TResult>(this Validation<TFailure, TSuccess> validation, Func<TSuccess, Task<Validation<TFailure, TResult>>> f)
        where TFailure : LanguageExt.Traits.Monoid<TFailure>
    {
        return await validation.Match(
            Fail: fail => Task.FromResult(Validation.Fail<TFailure, TResult>(fail)),
            Succ: f
        );
    }

    public static async Task<TMatch> MatchAsync<TSuccess, TMatch>(this Validation<Error, TSuccess> validation, Func<TSuccess, Task<TMatch>> onValue, Func<Error, TMatch> onFailure) =>
        await validation.Match(
            Fail: fail => Task.FromResult(onFailure(fail)),
            Succ: onValue
        );

    public static TResult MatchOrThrow<TSuccess, TResult>(this Validation<Error, TSuccess> validation, Func<TSuccess, TResult> convert) =>
        validation.Match(error => error.Throw().Return(ShouldNotBeCalled<TResult>), convert);

    private static TResult ShouldNotBeCalled<TResult>() => throw new ExceptionalException(nameof(ShouldNotBeCalled), -1_000_000_000);
}
