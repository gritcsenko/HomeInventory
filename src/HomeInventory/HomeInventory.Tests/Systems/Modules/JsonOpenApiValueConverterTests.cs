using Microsoft.OpenApi.Any;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public sealed class JsonOpenApiValueConverterTests() : BaseTest<JsonOpenApiValueConverterGivenContext>(t => new(t))
{
    [Fact]
    public void Convert_ShouldReturnNull_WhenValueIsNull()
    {
        Given
            .NullValue(out var valueVar)
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, valueVar, (sut, value) => sut.Convert(value, typeof(object)));

        then
            .Result(any => any.Should().BeOfType<OpenApiNull>());
    }

    [Fact]
    public void Convert_ShouldReturnOpenApiNull_WhenValueIsDbNull()
    {
        Given
            .DbNullValue(out var valueVar)
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, valueVar, (sut, value) => sut.Convert(value, value.GetType()));

        then
            .Result(any => any.Should().BeOfType<OpenApiNull>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Convert_ShouldReturnNull_WhenValueIsDbNull(bool expected)
    {
        Given
            .Value(out var valueVar, expected)
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, valueVar, (sut, value) => sut.Convert(value, value.GetType()));

        then
            .Result(any => any.Should().BeOfType<OpenApiBoolean>()
                .Which.Value.Should().Be(expected));
    }
}
