using FluentAssertions.Execution;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class FixedDateTimeServiceTests : BaseTest
{
    [Fact]
    public void UtcNow_ShouldReturnSuppliedTime()
    {
        var expected = DateTime.GetUtcNow();
        var sut = new FixedTimeProvider(DateTime);

        var actual = sut.GetUtcNow();

        actual.Should().Be(expected);
    }

    [Fact]
    public void UtcNow_ShouldReturnSuppliedTimeInUTC()
    {
        var expected = DateTime.GetUtcNow();
        var sut = new FixedTimeProvider(DateTime);

        var actual = sut.GetUtcNow();

        using var scope = new AssertionScope();
        actual.Should().Be(expected);
        actual.Offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void UtcNow_ShouldReturnSuppliedTimeFromOther()
    {
        var expected = DateTime.GetUtcNow();
        var other = new FixedTimeProvider(DateTime);
        var sut = new FixedTimeProvider(other);

        var actual = sut.GetUtcNow();

        using var scope = new AssertionScope();
        actual.Should().Be(expected);
        actual.Offset.Should().Be(TimeSpan.Zero);
    }
}
