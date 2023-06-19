namespace HomeInventory.Tests.Framework.Assertions;

[UnitTest]
public class ObjectExtensionsTests : BaseTest
{
    [Fact]
    public void GetComparer_ShouldReturnForValueType()
    {
        var value = Fixture.Create<Ulid>();
        var actual = ObjectExtensions.GetComparer<Ulid>();

        actual.Should().NotBeNull();
        actual(value, value).Should().BeTrue();
    }

    [Fact]
    public void GetComparer_ShouldReturnForReferenceType()
    {
        var value = Fixture.Create<string>();
        var actual = ObjectExtensions.GetComparer<string>();

        actual.Should().NotBeNull();
        actual(value, value).Should().BeTrue();
    }

    [Fact]
    public void GetComparer_ShouldReturnForObject()
    {
        var value = Fixture.Create<int>();
        var actual = ObjectExtensions.GetComparer<object>();

        actual.Should().NotBeNull();
        actual(value, (long)value).Should().BeTrue();
    }

    [Fact]
    public void GetComparer_ShouldReturnForNumeric()
    {
        var value = Fixture.Create<int>();
        var actual = ObjectExtensions.GetComparer<int>();

        actual.Should().NotBeNull();
        actual(value, value).Should().BeTrue();
    }
}
