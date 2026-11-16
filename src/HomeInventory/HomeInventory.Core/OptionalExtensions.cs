using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HomeInventory.Core;

public static class OptionalExtensions
{
    extension<T>(Optional<T> optional)
    {
        [Pure]
        public bool IsSome([NotNullWhen(true)] out T? value)
        {
            if (optional.Value is T t)
            {
                value = t;
                return true;
            }

            value = default;
            return false;
        }

        [Pure]
        public bool IsNone([NotNullWhen(false)] out T? value)
        {
            if (optional.Value is T t)
            {
                value = t;
                return false;
            }

            value = default;
            return true;
        }

        public async Task<Optional<T>> IfAsync(Func<T, CancellationToken, Task<bool>> asyncCondition, CancellationToken cancellationToken = default) =>
            optional.IsSome(out var some) && await asyncCondition(some, cancellationToken)
                ? some
                : None.Value;
    }

    extension<T>(Task<Optional<T>> optionalTask)
    {
        public async Task<Optional<TResult>> SelectAsync<TResult>(Func<T, CancellationToken, Task<TResult>> asyncSelector, CancellationToken cancellationToken = default)
        {
            var optional = await optionalTask;
            if (optional.IsSome(out var some))
            {
                return await asyncSelector(some, cancellationToken);
            }

            return None.Value;
        }
    }

    public static Optional<T> Flatten<T>(this Optional<Optional<T>> optional) =>
        optional.SelectMany(static x => x);

    public static Optional<T> NoneIfNull<T>(this T? value)
        where T : class =>
        value is not null ? value : Optional<T>.None;

    public static Validated<T, Error> ErrorIfNone<T>(this Optional<Validated<T, Error>> option, Func<Error> errorFactory) =>
        option.IfNone(() => errorFactory());

    public static Validated<T, Error> ErrorIfNone<T>(this Optional<T> option, Error error) =>
        option.Select(x => (Validated<T, Error>)x).IfNone(error);

    public static Optional<T> ToOptional<T>(this T? nullable)
        where T : struct =>
        nullable is not null ? nullable.Value : Optional<T>.None;

    public static Optional<T> ToOptional<T>(this IEnumerable<T> enumerable)
    {
        using (var enumerator = enumerable.GetEnumerator())
        {
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
        }

        return None.Value;
    }
}
