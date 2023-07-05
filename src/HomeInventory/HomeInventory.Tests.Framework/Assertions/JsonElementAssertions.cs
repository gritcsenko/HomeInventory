using System.Text.Json;
using HomeInventory.Core;
using HomeInventory.Tests.Framework.Assertions;

namespace HomeInventory.Tests.Framework.Assertions;

public class JsonElementAssertions : ObjectAssertions<JsonElement, JsonElementAssertions>
{
    public JsonElementAssertions(JsonElement value)
        : base(value)
    {
    }

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
