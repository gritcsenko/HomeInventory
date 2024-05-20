using FluentAssertions.Execution;
using HomeInventory.Domain;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class FixedDateTimeServiceTests : BaseTest
{
    [Fact]
    public void UtcNow_ShouldReturnSuppliedTime()
    {
        var parent = new FixedTimeProvider(TimeProvider.System);
        var expected = parent.GetUtcNow();
        var sut = new FixedTimeProvider(parent);

        var actual = sut.GetUtcNow();

        actual.Should().Be(expected);
    }

    [Fact]
    public void UtcNow_ShouldReturnSuppliedTimeInUTC()
    {
        var parent = new FixedTimeProvider(TimeProvider.System);
        var expected = parent.GetUtcNow();
        var sut = new FixedTimeProvider(parent);

        var actual = sut.GetUtcNow();

        using var scope = new AssertionScope();
        actual.Should().Be(expected);
        actual.Offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void UtcNow_ShouldReturnSuppliedTimeFromOther()
    {
        var parent = new FixedTimeProvider(TimeProvider.System);
        var expected = parent.GetUtcNow();
        var other = new FixedTimeProvider(parent);
        var sut = new FixedTimeProvider(other);

        var actual = sut.GetUtcNow();

        using var scope = new AssertionScope();
        actual.Should().Be(expected);
        actual.Offset.Should().Be(TimeSpan.Zero);
    }
}
