using System.Text.Json;

namespace HomeInventory.Tests.Framework.Assertions;

[UnitTest]
public class JsonElementAssertionsTests : BaseTest
{
    private const string _rootProperty = "key";

    [Fact]
    public void BeArrayEqualTo_ShouldPass_WhenBothNull()
    {
        using var document = JsonDocument.Parse($$"""{"{{_rootProperty}}":null}""");
        var sut = CreateSut(document, _rootProperty);

        Action action = () => sut.BeArrayEqualTo(null);

        action.Should().NotThrow();
    }

    [Fact]
    public void BeArrayEqualTo_ShouldPass_WhenBothEmpty()
    {
        using var document = JsonDocument.Parse($$"""{"{{_rootProperty}}":[]}""");
        var sut = CreateSut(document, _rootProperty);

        Action action = () => sut.BeArrayEqualTo([]);

        action.Should().NotThrow();
    }

    [Fact]
    public void BeArrayEqualTo_ShouldPass_WhenBothHasSameValue()
    {
        var expected = new[] { "value" };
        using var document = JsonDocument.Parse($$"""{"{{_rootProperty}}":["{{expected[0]}}"]}""");
        var sut = CreateSut(document, _rootProperty);

        Action action = () => sut.BeArrayEqualTo(expected);

        action.Should().NotThrow();
    }

    private static JsonElementAssertions CreateSut(JsonDocument document, string rootProperty) => document.RootElement.GetProperty(rootProperty).Should();
}
