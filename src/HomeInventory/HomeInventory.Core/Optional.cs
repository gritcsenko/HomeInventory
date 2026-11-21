using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;

namespace HomeInventory.Core;

[JsonUnion]
[SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "By design")]
public readonly union Optional<T>(T, None) : IEquatable<Optional<T>>, IReadOnlyCollection<T>
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "Reduce consumer code")]
    public static None None { get; } = None.Value;

    public bool IsNone =>
        Value is None;

    public bool IsSome =>
        Value is T;

    public int Count => IsSome ? 1 : 0;

    public static bool operator==(Optional<T> a, Optional<T> b) =>
        a.Equals(b);

    public static bool operator!=(Optional<T> a, Optional<T> b) =>
        !a.Equals(b);

    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "IsNone/IsSome are present")]
    public static bool operator true(Optional<T> optional) => optional != None;

    public static bool operator false(Optional<T> optional) => optional == None;

    public IEnumerator<T> GetEnumerator() => AsEnumerable().GetEnumerator();

    [Pure]
    public override bool Equals(object? obj) =>
        obj is Optional<T> option && Equals(option);

    [Pure]
    public override int GetHashCode() =>
        (Value != null ? Value.GetHashCode() : 0);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [Pure]
    public bool Equals(Optional<T> other) =>
        other.Value?.Equals(Value) == true;

    [Pure]
    public Optional<T> Do(Action<T> action)
    {
        if (this.IsSome(out var some))
        {
            action(some);
        }

        return this;
    }

    [Pure]
    public Optional<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        if (this.IsSome(out var some))
        {
            return selector(some);
        }

        return None;
    }

    public Optional<TResult> SelectMany<TOther, TResult>(Func<T, Optional<TOther>> selector, Func<T, TOther, TResult> resultSelector)
    {
        if (this.IsSome(out var some))
        {
            return selector(some).Select(o => resultSelector(some, o));
        }

        return None;
    }

    public Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> selector)
    {
        if (this.IsSome(out var some))
        {
            return selector(some);
        }

        return None;
    }

    [Pure]
    public T IfNone(Func<T> onNone) =>
        this.IsNone(out var some) ? onNone() : some;

    [Pure]
    public T IfNone(T onNone) =>
        this.IsNone(out var some) ? onNone : some;

    public void IfNone(Action onNone)
    {
        if (IsNone)
        {
            onNone();
        }
    }

    public void IfSome(Action<T> onSome)
    {
        if (this.IsSome(out var some))
        {
            onSome(some);
        }
    }

    [Pure]
    public IEnumerable<T> AsEnumerable()
    {
        if (this.IsSome(out var some))
        {
            yield return some;
        }
    }

    [Pure]
    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) =>
        this.IsSome(out var some) ? onSome(some) : onNone();

    [Pure]
    public TResult Match<TResult>(Func<T, TResult> onSome, TResult onNone) =>
        this.IsSome(out var some) ? onSome(some) : onNone;

    public void Match(Action<T> onSome, Action onNone)
    {
        if (this.IsSome(out var some))
        {
            onSome(some);
        }
        else
        {
            onNone();
        }
    }

    [Pure]
    public Optional<T> Coalesce(Func<Optional<T>> secondFunc) =>
        this.IsSome(out var some) ? some : secondFunc();

    [Pure]
    public Optional<T> Where(Func<T, bool> predicate) =>
        this.IsSome(out var some) && predicate(some) ? some : None;

    public T ThrowIfNone(Func<Exception> exceptionFactory) =>
        IfNone(() => exceptionFactory().Rethrow<T>());
}
