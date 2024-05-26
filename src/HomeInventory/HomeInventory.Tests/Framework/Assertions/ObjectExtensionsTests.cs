using FluentAssertions.Execution;
using Visus.Cuid;

namespace HomeInventory.Tests.Framework.Assertions;

[UnitTest]
public class ObjectExtensionsTests : BaseTest
{
    [Fact]
    public void GetComparer_ShouldReturnForValueType()
    {
        Fixture.CustomizeCuid();
        var value = Fixture.Create<Cuid>();
        var actual = ObjectExtensions.GetComparer<Cuid>();

        using var scope = new AssertionScope();
        actual.Should().NotBeNull();
        actual(value, value).Should().BeTrue();
    }

    [Fact]
    public void GetComparer_ShouldReturnForReferenceType()
    {
        var value = Fixture.Create<string>();
        var actual = ObjectExtensions.GetComparer<string>();

        using var scope = new AssertionScope();
        actual.Should().NotBeNull();
        actual(value, value).Should().BeTrue();
    }

    [Fact]
    public void GetComparer_ShouldReturnForObject()
    {
        var value = Fixture.Create<int>();
        var actual = ObjectExtensions.GetComparer<object>();

        using var scope = new AssertionScope();
        actual.Should().NotBeNull();
        actual(value, (long)value).Should().BeTrue();
    }

    [Fact]
    public void GetComparer_ShouldReturnForNumeric()
    {
        var value = Fixture.Create<int>();
        var actual = ObjectExtensions.GetComparer<int>();

        using var scope = new AssertionScope();
        actual.Should().NotBeNull();
        actual(value, value).Should().BeTrue();
    }
}
