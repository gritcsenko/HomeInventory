using System.Collections;
using System.Globalization;
using FluentAssertions.Execution;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Framework.Assertions;

public class DictionaryAssertions : ReferenceTypeAssertions<IDictionary, DictionaryAssertions>
{
    public DictionaryAssertions(IDictionary subject)
        : base(subject) =>
        Identifier = Subject.GetType().GetFormattedName();

    protected override string Identifier { get; }

    /// <summary>
    /// Asserts that the current dictionary contains the specified <paramref name="value" /> for the supplied
    /// <paramref name="key" />.
    /// Key comparison will honor the equality comparer of the dictionary when applicable.
    /// Values are compared using their <see cref="object.Equals(object)" /> implementation.
    /// </summary>
    /// <param name="key">The key for which to validate the value</param>
    /// <param name="value">The value to validate</param>
    /// <param name="because">
    /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">
    /// Zero or more objects to format using the placeholders in <paramref name="because" />.
    /// </param>
    public AndConstraint<DictionaryAssertions> Contain(object key, object value,
        string because = "", params object[] becauseArgs)
    {
        bool success = Execute.Assertion
            .ForCondition(Subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:dictionary} to contain value {0} at key {1}{reason}, but dictionary is <null>.",
                value, key);

        if (success)
        {
            if (TryGetValue(Subject!, key, out var actual))
            {
                var areSameOrEqual = ObjectExtensions.GetComparer<object>();

                Execute.Assertion
                    .ForCondition(areSameOrEqual(actual!, value))
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:dictionary} to contain value {0} at key {1}{reason}, but found {2}.", value, key, actual);
            }
            else
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:dictionary} to contain value {0} at key {1}{reason}, but the key was not found.", value, key);
            }
        }

        return new AndConstraint<DictionaryAssertions>(this);
    }

    private static bool TryGetValue(IDictionary collection, object key, out object? value)
    {
        if (collection.Contains(key))
        {
            value = collection[key];
            return true;
        }

        value = null;
        return false;
    }
}

internal static class ObjectExtensions
{
    public static Func<T, T, bool> GetComparer<T>()
    {
        if (typeof(T).IsValueType)
        {
            // Avoid causing any boxing for value types
            return (actual, expected) => EqualityComparer<T>.Default.Equals(actual, expected);
        }

        if (typeof(T) != typeof(object))
        {
            // CompareNumerics is only relevant for numerics boxed in an object.
            return (actual, expected) => actual is null
                ? expected is null
                : expected is not null && EqualityComparer<T>.Default.Equals(actual, expected);
        }

        return (actual, expected) => actual is null
            ? expected is null
            : expected is not null
            && (EqualityComparer<T>.Default.Equals(actual, expected) || CompareNumerics(actual, expected));
    }

    private static bool CompareNumerics(object actual, object expected)
    {
        var expectedType = expected.GetType();
        var actualType = actual.GetType();

        return actualType != expectedType
            && actual.IsNumericType()
            && expected.IsNumericType()
            && CanConvert(actual, expected, actualType, expectedType)
            && CanConvert(expected, actual, expectedType, actualType);
    }

    private static bool CanConvert(object source, object target, Type sourceType, Type targetType)
    {
        try
        {
            var converted = source.ConvertTo(targetType);

            return source.Equals(converted.ConvertTo(sourceType))
                && converted.Equals(target);
        }
        catch
        {
            // ignored
            return false;
        }
    }

    private static object ConvertTo(this object source, Type targetType)
    {
        return Convert.ChangeType(source, targetType, CultureInfo.InvariantCulture);
    }

    private static bool IsNumericType(this object obj)
    {
        // "is not null" is due to https://github.com/dotnet/runtime/issues/47920#issuecomment-774481505
        return obj is not null and (
            int or
            long or
            float or
            double or
            decimal or
            sbyte or
            byte or
            short or
            ushort or
            uint or
            ulong);
    }
}
