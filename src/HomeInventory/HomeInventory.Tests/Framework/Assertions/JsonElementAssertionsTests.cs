using System.Text.Json;

namespace HomeInventory.Tests.Framework.Assertions;

[UnitTest]
public class JsonElementAssertionsTests : BaseTest
{
    [Fact]
    public void BeArrayEqualTo_ShoudPass_WhenBothNull()
    {
        var rootProperty = "key";
        using var document = JsonDocument.Parse($$"""{"{{rootProperty}}":null}""");
        var sut = CreateSut(document, rootProperty);

        Action action = () => sut.BeArrayEqualTo(null);

        action.Should().NotThrow();
    }

    [Fact]
    public void BeArrayEqualTo_ShoudPass_WhenBothEmpty()
    {
        var rootProperty = "key";
        using var document = JsonDocument.Parse($$"""{"{{rootProperty}}":[]}""");
        var sut = CreateSut(document, rootProperty);

        Action action = () => sut.BeArrayEqualTo([]);

        action.Should().NotThrow();
    }

    [Fact]
    public void BeArrayEqualTo_ShoudPass_WhenBothHasSameValue()
    {
        var rootProperty = "key";
        var expected = new[] { "value" };
        using var document = JsonDocument.Parse($$"""{"{{rootProperty}}":["{{expected[0]}}"]}""");
        var sut = CreateSut(document, rootProperty);

        Action action = () => sut.BeArrayEqualTo(expected);

        action.Should().NotThrow();
    }

    private static JsonElementAssertions CreateSut(JsonDocument document, string rootProperty) => new(document.RootElement.GetProperty(rootProperty));
}
