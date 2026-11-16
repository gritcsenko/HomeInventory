using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HomeInventory.Core;

public readonly union Validated<T, TError>(T, TError) : IEquatable<Validated<T, TError>>
{
    public bool IsSuccess => this.Value is T;

    public bool IsFail => this.Value is TError;

    public static bool operator==(Validated<T, TError> a, Validated<T, TError> b) =>
        a.Equals(b);

    public static bool operator!=(Validated<T, TError> a, Validated<T, TError> b) =>
        !a.Equals(b);

    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "IsSuccess/IsFail are present")]
    public static bool operator true(Validated<T, TError> validated) => validated.IsSuccess;

    public static bool operator false(Validated<T, TError> validated) => validated.IsFail;

    [Pure]
    public Validated<TResult, TError> Select<TResult>(Func<T, TResult> selector)
    {
        if (this.IsSuccess(out var success, out var error))
        {
            return selector(success);
        }

        return error;
    }

    [Pure]
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<TError, TResult> onError) =>
        this.IsSuccess(out var success, out var error) switch
        {
            true => onSuccess(success),
            false => onError(error),
        };

    public TError ThrowIfSuccess(Func<T, Exception> onSuccess)
    {
        if (this.IsSuccess(out var success, out var error))
        {
            onSuccess(success).Rethrow();
        }

        return error;
    }

    public T ThrowIfFailed(Func<TError, Exception> onError)
    {
        if (this.IsSuccess(out var success, out var error))
        {
            return success;
        }

        onError(error).Rethrow();
        throw new InvalidOperationException();
    }

    public Validated<T, TError> IfSuccess(Action<T> onSuccess)
    {
        if (this.IsSuccess(out var success, out _))
        {
            onSuccess(success);
        }

        return this;
    }

    public bool Equals(Validated<T, TError> other) => Equals(Value, other.Value);

    public override bool Equals(object? obj) => obj is Validated<T, TError> other && Equals(other);

    public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

    public Optional<T> SuccessOrNone() =>
        this.IsSuccess(out var success, out _)
            ? success
            : None.Value;

    public Optional<TError> FailureOrNone() =>
        this.IsSuccess(out _, out var error)
            ? None.Value
            : error;
}
