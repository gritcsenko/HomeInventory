using System.Text.Json;

namespace HomeInventory.Tests.Framework.Assertions;

public class JsonElementAssertions(JsonElement value) : ObjectAssertions<JsonElement, JsonElementAssertions>(value)
{
    public AndConstraint<JsonElementAssertions> BeArrayEqualTo(IReadOnlyCollection<string>? items, string because = "", params object[] becauseArgs) =>
        BeArrayEqualTo(items, (actual, expected) => actual.GetString().Should().Be(expected, because, becauseArgs), because, becauseArgs);

    public AndConstraint<JsonElementAssertions> BeArrayEqualTo<T>(IReadOnlyCollection<T>? items, Action<JsonElement, T> assert, string because = "", params object[] becauseArgs)
    {
        if (items is null)
        {
            Subject.ValueKind.Should().Be(JsonValueKind.Null, because, becauseArgs);
            return new(this);
        }

        ShouldBeArray(because, becauseArgs);
        ShouldHanveCount(items.Count, because, becauseArgs);

        Subject.EnumerateArray().Zip(items).Iter(tuple => assert(tuple.Item1, tuple.Item2));

        return new(this);
    }

    public AndConstraint<JsonElementAssertions> BeArray(Action<JsonElement> assert, string because = "", params object[] becauseArgs)
    {
        ShouldBeArray(because, becauseArgs);
        Subject.EnumerateArray().Iter(assert);

        return new(this);
    }

    public AndWhichConstraint<JsonElementAssertions, JsonElement> BeObject(string because = "", params object[] becauseArgs)
    {
        ShouldBeOfKind(JsonValueKind.Object, because, becauseArgs);

        return new(this, Subject);
    }

    public AndWhichConstraint<JsonElementAssertions, JsonElement> HaveProperty(string propertyName, string because = "", params object[] becauseArgs)
    {
        var hasProperty = Subject.TryGetProperty(propertyName, out var propertyValue);
        FluentAssertions.Execution.Execute.Assertion
            .ForCondition(hasProperty)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context} to contain {0} property{reason}.", propertyName);

        return new(this, propertyValue);
    }

    public AndWhichConstraint<JsonElementAssertions, string> HaveValue(string value, string because = "", params object[] becauseArgs)
    {
        ShouldBeOfKind(JsonValueKind.String, because, becauseArgs);

        var sameValue = Subject.ValueEquals(value);
        FluentAssertions.Execution.Execute.Assertion
            .ForCondition(sameValue)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context} to contain {0} value{reason}, but found {1}.", Subject.GetString());

        return new(this, value);
    }

    private void ShouldHanveCount(int expectedCount, string because, object[] becauseArgs)
    {
        var actualCount = Subject.GetArrayLength();
        FluentAssertions.Execution.Execute.Assertion
            .ForCondition(actualCount == expectedCount)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context} to have an array with {0} item(s){reason}, but found {1}: {2}.", expectedCount, actualCount, Subject);
    }

    private void ShouldBeArray(string because, object[] becauseArgs) => ShouldBeOfKind(JsonValueKind.Array, because, becauseArgs);

    private void ShouldBeOfKind(JsonValueKind kind, string because, object[] becauseArgs) => Subject.ValueKind.Should().Be(kind, because, becauseArgs);
}
