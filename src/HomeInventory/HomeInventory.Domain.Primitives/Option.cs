﻿namespace HomeInventory.Domain.Primitives;

public static class Option
{
    public static Option<T> None<T>()
        where T : notnull =>
        Option<T>.None();

    public static Option<T> Some<T>(this T value)
        where T : notnull =>
        Option<T>.Some(value);

    public static bool IsNone<T>(this Option<T> option)
        where T : notnull =>
        option.When(false, true);

    public static bool IsSome<T>(this Option<T> option)
        where T : notnull =>
        option.When(true, false);

    public static Option<T> Unwrap<T>(this Option<Option<T>> option)
        where T : notnull =>
        option.Reduce(None<T>);

    public static Option<T> ToOption<T>(this T? content)
        where T : notnull =>
        content is null ? None<T>() : content.Some();

    public static T Reduce<T>(this Option<T> option, T orElse)
        where T : notnull =>
        option.When(c => c, orElse);

    public static T? ReduceNullable<T>(this Option<T> option, T? orElse = default)
        where T : notnull =>
        option.When(c => c, orElse);

    public static T Reduce<T>(this Option<T> option, Func<T> orElse)
        where T : notnull =>
        option.When(c => c, orElse);

    public static Option<TResult> Select<T, TResult>(this Option<T> option, Func<T, Option<TResult>> selector)
        where T : notnull
        where TResult : notnull =>
        option.When(c => selector(c), Option<TResult>.None);

    public static Option<TResult> Select<T, TResult>(this Option<T> option, Func<T, TResult?> selector)
        where T : notnull
        where TResult : notnull =>
        option.Select(c => selector(c).ToOption());

    public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
        where T : notnull =>
        option.Select(c => predicate(c) ? c.Some() : None<T>());

    public static Option<T> WhereNot<T>(this Option<T> option, Func<T, bool> predicate)
        where T : notnull =>
        option.Select(c => !predicate(c) ? c.Some() : None<T>());
}

public class Option<T> : Equatable<Option<T>>
    where T : notnull
{
    private static readonly Option<T> _none = new();
    private readonly T? _content;
    private readonly bool _hasValue;

    private Option()
    {
        _content = default;
        _hasValue = false;
    }

    private Option(T content)
        : base(content, true) =>
        (_content, _hasValue) = (content, true);

    public static Option<T> None() =>
        _none;

    public static Option<T> Some(T content)
    {
        ArgumentNullException.ThrowIfNull(content);
        return new(content);
    }

    public TResult When<TResult>(Func<T, TResult> content, Func<TResult> noContent) =>
        _hasValue && _content is not null
        ? content(_content)
        : noContent();

    public TResult When<TResult>(Func<T, TResult> content, TResult noContent) =>
        _hasValue && _content is not null
        ? content(_content)
        : noContent;

    public TResult When<TResult>(TResult content, TResult noContent) =>
        _hasValue && _content is not null
        ? content
        : noContent;

    public override string? ToString() =>
        this.Select(c => c.ToString().ToOption()).When(c => c, default(string?));
}
