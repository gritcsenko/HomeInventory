namespace HomeInventory.Tests.Core;

[UnitTest]
public sealed class NoneTests
{
    [Fact]
    public void Equals_ShouldReturnTrue_WhenComparedWithNone()
    {
        var sut = None.Value;
        var other = default(None);

        using var scope = new AssertionScope();
        sut.Equals(other).Should().BeTrue();
        sut.Equals((object)other).Should().BeTrue();
        (sut == other).Should().BeTrue();
        (sut != other).Should().BeFalse();
        sut.GetHashCode().Should().Be(other.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparedWithOtherType()
    {
        var sut = None.Value;

        using var scope = new AssertionScope();
        sut.Equals(new object()).Should().BeFalse();
        sut.Equals(null).Should().BeFalse();
    }
}
