namespace HomeInventory.Core;

public static class ValidationExtensions
{
    public static async Task<Validation<TFailure, TResult>> MapAsync<TFailure, TSuccess, TResult>(this Validation<TFailure, TSuccess> validation, Func<TSuccess, Task<TResult>> f)
        => await validation.MapAsync(async s => Validation<TFailure, TResult>.Success(await f(s)));

    public static async Task<Validation<TFailure, TResult>> MapAsync<TFailure, TSuccess, TResult>(this Validation<TFailure, TSuccess> validation, Func<TSuccess, Task<Validation<TFailure, TResult>>> f)
    {
        if (validation.IsSuccess)
        {
            return await f((TSuccess)validation);
        }

        return (Seq<TFailure>)validation;
    }
}
