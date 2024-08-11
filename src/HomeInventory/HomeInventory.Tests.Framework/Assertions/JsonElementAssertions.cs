using System.Text.Json;
using HomeInventory.Tests.Framework.Assertions;

namespace HomeInventory.Tests.Framework.Assertions;

public class JsonElementAssertions(JsonElement value) : ObjectAssertions<JsonElement, JsonElementAssertions>(value)
{
    public void BeArrayEqualTo(IReadOnlyCollection<string>? items)
    {
        if (items is null)
        {
            Subject.ValueKind.Should().Be(JsonValueKind.Null);
            return;
        }

        Subject.ValueKind.Should().Be(JsonValueKind.Array);
        Subject.GetArrayLength().Should().Be(items.Count);
        Subject.EnumerateArray().Zip(items, (actual, expected) =>
            actual.GetString().Should().Be(expected)).ToReadOnly();
    }
}
