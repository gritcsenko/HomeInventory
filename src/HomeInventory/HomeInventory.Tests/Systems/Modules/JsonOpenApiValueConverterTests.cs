﻿using Microsoft.OpenApi.Any;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public sealed class JsonOpenApiValueConverterTests() : BaseTest<JsonOpenApiValueConverterGivenContext>(static t => new(t))
{
    [Fact]
    public void Convert_ShouldReturnNull_WhenValueIsNull()
    {
        Given
            .NullValue(out var value)
            .Sut(out var sut);

        var then = When
            .Invoked(sut, value, static (sut, value) => sut.Convert(value, typeof(object)));

        then
            .Result(static any => any.Should().BeOfType<OpenApiNull>());
    }

    [Fact]
    public void Convert_ShouldReturnOpenApiNull_WhenValueIsDbNull()
    {
        Given
            .DbNullValue(out var value)
            .Sut(out var sut);

        var then = When
            .Invoked(sut, value, static (sut, value) => sut.Convert(value, value.GetType()));

        then
            .Result(static any => any.Should().BeOfType<OpenApiNull>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Convert_ShouldReturnNull_WhenValueIsDbNull(bool expected)
    {
        Given
            .Value(out var value, expected)
            .Sut(out var sut);

        var then = When
            .Invoked(sut, value, (sut, value) => sut.Convert(value, value.GetType()));

        then
            .Result(any => any.Should().BeOfType<OpenApiBoolean>()
                .Which.Value.Should().Be(expected));
    }
}
