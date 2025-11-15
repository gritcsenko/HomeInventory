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
        where TFailure : LanguageExt.Traits.Monoid<TFailure> =>
        await validation.Match(
            Fail: fail => Task.FromResult(Validation.Fail<TFailure, TResult>(fail)),
            Succ: async succ => Validation.Success<TFailure, TResult>(await f(succ))
        );

    extension<TSuccess>(Validation<Error, TSuccess> validation)
    {
        public async Task<TMatch> MatchAsync<TMatch>(Func<TSuccess, Task<TMatch>> onValue, Func<Error, TMatch> onFailure) =>
            await validation.Match(
                Fail: fail => Task.FromResult(onFailure(fail)),
                Succ: onValue
            );

        public TResult MatchOrThrow<TResult>(Func<TSuccess, TResult> convert) =>
            validation.Match(error => error.Throw().Return(ShouldNotBeCalled<TResult>), convert);
    }

    private static TResult ShouldNotBeCalled<TResult>() => throw new ExceptionalException(nameof(ShouldNotBeCalled), -1_000_000_000);
}
