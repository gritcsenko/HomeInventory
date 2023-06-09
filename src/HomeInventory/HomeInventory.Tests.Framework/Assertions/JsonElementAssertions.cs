using System.Text.Json;
using HomeInventory.Tests.Framework.Assertions;

namespace HomeInventory.Tests.Framework.Assertions;

public class JsonElementAssertions : ObjectAssertions<JsonElement, JsonElementAssertions>
{
    public JsonElementAssertions(JsonElement value)
        : base(value)
    {
    }

    public void BeArrayEqualTo(params string[] items)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        Subject.ValueKind.Should().Be(JsonValueKind.Array);
        Subject.GetArrayLength().Should().Be(items.Length);
        var subElements = Subject.EnumerateArray().ToArray();
        for (var i = 0; i < items.Length; i++)
        {
            subElements[i].GetString().Should().Be(items[i]);
        }
    }
}
