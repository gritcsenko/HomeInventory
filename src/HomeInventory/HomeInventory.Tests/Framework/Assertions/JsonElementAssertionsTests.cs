using System.Text.Json;

namespace HomeInventory.Tests.Framework.Assertions;

[UnitTest]
public class JsonElementAssertionsTests : BaseTest
{
    [Fact]
    public void BeArrayEqualTo_ShoudPass_WhenBothNull()
    {
        using var document = JsonDocument.Parse("{\"key\":null}");
        var element = document.RootElement.GetProperty("key");
        var sut = new JsonElementAssertions(element);

        Action action = () => sut.BeArrayEqualTo(null);

        action.Should().NotThrow();
    }

    [Fact]
    public void BeArrayEqualTo_ShoudPass_WhenBothEmpty()
    {
        using var document = JsonDocument.Parse("{\"key\":[]}");
        var element = document.RootElement.GetProperty("key");
        var sut = new JsonElementAssertions(element);

        Action action = () => sut.BeArrayEqualTo([]);

        action.Should().NotThrow();
    }

    [Fact]
    public void BeArrayEqualTo_ShoudPass_WhenBothHasSameValue()
    {
        var expected = new[] { "value" };
        using var document = JsonDocument.Parse("{\"key\":[\"value\"]}");
        var element = document.RootElement.GetProperty("key");
        var sut = new JsonElementAssertions(element);

        Action action = () => sut.BeArrayEqualTo(expected);

        action.Should().NotThrow();
    }
}
