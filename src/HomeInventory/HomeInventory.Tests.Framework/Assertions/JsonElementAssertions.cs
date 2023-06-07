using System.Text.Json;
using HomeInventory.Tests.Framework.Assertions;

namespace HomeInventory.Tests.Framework.Assertions;

internal class JsonElementAssertions : ObjectAssertions<JsonElement, JsonElementAssertions>
{
    public JsonElementAssertions(JsonElement value)
        : base(value)
    {
    }

    public void BeArrayEqualTo(string[] items)
    {
        Subject.ValueKind.Should().Be(JsonValueKind.Array);
        Subject.GetArrayLength().Should().Be(items.Length);
        var subElements = Subject.EnumerateArray().ToArray();
        for (var i = 0; i < items.Length; i++)
        {
            subElements[i].GetString().Should().Be(items[i]);
        }
    }
}
