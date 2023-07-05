using System.Collections;
using HomeInventory.Core;
using Execute = FluentAssertions.Execution.Execute;

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
            .FailWith("Expected {context:dictionary} to contain value {0} at key {1}{reason}, but dictionary is <null>.", value, key);

        if (success)
        {
            if (TryGetValue(Subject!, key, out var actual))
            {
                var areSameOrEqual = ObjectExtensions.GetComparer<object>();

                Execute.Assertion
                    .ForCondition(areSameOrEqual(actual, value))
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
