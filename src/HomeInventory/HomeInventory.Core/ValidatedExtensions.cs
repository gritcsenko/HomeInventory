using System.Diagnostics.CodeAnalysis;

namespace HomeInventory.Core;

public static class ValidatedExtensions
{
    extension<T, TError>(Validated<T, TError> validated)
    {
        public bool IsSuccess([NotNullWhen(true)] out T? success, [NotNullWhen(false)]out TError? error)
        {
            success = default;
            error = default;
            switch (validated)
            {
                case T value:
                    success = value;
                    return true;
                case TError err:
                    error = err;
                    return false;
            }

            throw  new InvalidOperationException();
        }

        public async Task<Validated<TResult, TError>> SelectAsync<TResult>(Func<T, Task<TResult>> selector)
        {
            if (validated.IsSuccess(out var success, out var error))
            {
                return await selector(success);
            }

            return error;
        }

        public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> onSuccess, Func<TError, TResult> onError) =>
            validated.IsSuccess(out var success, out var error) switch
            {
                true => await onSuccess(success),
                false => onError(error),
            };
    }
}
