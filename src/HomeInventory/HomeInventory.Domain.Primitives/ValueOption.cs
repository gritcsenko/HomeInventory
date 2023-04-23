namespace HomeInventory.Domain.Primitives;

public static class ValueOption
{
    public static ValueOption<T> None<T>()
        where T : struct =>
        ValueOption<T>.None();

    public static ValueOption<T> Some<T>(this T value)
        where T : struct =>
        ValueOption<T>.Some(value);

    public static ValueOption<T> Unwrap<T>(this ValueOption<ValueOption<T>> option)
        where T : struct =>
        option.Reduce(None<T>);

    public static ValueOption<T> ToOption<T>(this T? content)
        where T : struct =>
        content is null ? None<T>() : Some(content.Value);

    public static T Reduce<T>(this ValueOption<T> option, T orElse)
        where T : struct =>
        option.When(c => c, orElse);

    public static T? Reduce<T>(this ValueOption<T> option, T? orElse = default)
        where T : struct =>
        option.When(c => c, orElse);

    public static T Reduce<T>(this ValueOption<T> option, Func<T> orElse)
        where T : struct =>
        option.When(c => c, orElse);

    public static Option<TResult> Select<T, TResult>(this ValueOption<T> option, Func<T, Option<TResult>> selector)
        where T : struct
        where TResult : notnull =>
        option.When(c => selector(c), Option<TResult>.None);

    public static Option<TResult> Select<T, TResult>(this ValueOption<T> option, Func<T, TResult?> selector)
        where T : struct
        where TResult : notnull =>
        option.Select(c => selector(c).ToOption());

    public static ValueOption<TResult> Select<T, TResult>(this ValueOption<T> option, Func<T, ValueOption<TResult>> selector)
        where T : struct
        where TResult : struct =>
        option.When(c => selector(c), ValueOption<TResult>.None);

    public static ValueOption<TResult> Select<T, TResult>(this ValueOption<T> option, Func<T, TResult?> selector)
        where T : struct
        where TResult : struct =>
        option.Select(c => selector(c).ToOption());

    public static ValueOption<T> Where<T>(this T? content, Func<T, bool> predicate)
        where T : struct =>
        content.ToOption().Where(predicate);

    public static ValueOption<T> Where<T>(this ValueOption<T> option, Func<T, bool> predicate)
        where T : struct =>
        option.Select(c => predicate(c) ? Some(c) : None<T>());

    public static ValueOption<T> WhereNot<T>(this T? content, Func<T, bool> predicate)
        where T : struct =>
        content.ToOption().WhereNot(predicate);

    public static ValueOption<T> WhereNot<T>(this ValueOption<T> option, Func<T, bool> predicate)
        where T : struct =>
        option.Select(c => !predicate(c) ? Some(c) : None<T>());
}

public readonly struct ValueOption<T> : IEquatable<ValueOption<T>>
    where T : struct
{
    private readonly EquatableComponent<ValueOption<T>> _equatable;
    private readonly T? _content;

    public ValueOption()
    {
        _equatable = new EquatableComponent<ValueOption<T>>();
    }

    private ValueOption(T content)
    {
        _content = content;
        _equatable = new EquatableComponent<ValueOption<T>>(_content);
    }

    public static ValueOption<T> None() => new();

    public static ValueOption<T> Some(T content) => new(content);

    public TResult When<TResult>(Func<T, TResult> content, Func<TResult> noContent) =>
        _content is null
        ? noContent()
        : content(_content.Value);

    public TResult When<TResult>(Func<T, TResult> content, TResult noContent) =>
        _content is null
        ? noContent
        : content(_content.Value);

    public TResult When<TResult>(TResult content, TResult noContent) =>
        _content is null
        ? noContent
        : content;

    public bool Equals(ValueOption<T> other) => _equatable.Equals(other._equatable);

    public static bool operator ==(ValueOption<T> left, ValueOption<T> right) => left.Equals(right);

    public static bool operator !=(ValueOption<T> left, ValueOption<T> right) => !left.Equals(right);

    public override bool Equals(object? obj) => obj is ValueOption<T> o && Equals(o);

    public override int GetHashCode() => _equatable.GetHashCode();

    public override string? ToString() => this.Select(c => c.ToString().ToOption()).ReduceNullable();
}
