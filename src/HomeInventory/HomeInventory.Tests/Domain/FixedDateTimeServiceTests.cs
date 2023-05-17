using HomeInventory.Domain;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class FixedDateTimeServiceTests : BaseTest
{
    [Fact]
    public void UtcNow_ShouldReturnSuppliedTime()
    {
        var expected = DateTimeOffset.UtcNow;
        var sut = new FixedDateTimeService(expected);

        var actual = sut.UtcNow;

        actual.Should().Be(expected);
    }

    [Fact]
    public void UtcNow_ShouldReturnSuppliedTimeInUTC()
    {
        var expected = DateTimeOffset.UtcNow;
        var sut = new FixedDateTimeService(expected.ToOffset(TimeSpan.FromHours(12)));

        var actual = sut.UtcNow;

        actual.Should().Be(expected);
        actual.Offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void UtcNow_ShouldReturnSuppliedTimeFromOther()
    {
        var expected = DateTimeOffset.UtcNow;
        var other = new FixedDateTimeService(expected);
        var sut = new FixedDateTimeService(other);

        var actual = sut.UtcNow;

        actual.Should().Be(expected);
        actual.Offset.Should().Be(TimeSpan.Zero);
    }
}
