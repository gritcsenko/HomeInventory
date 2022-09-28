using AutoFixture;
using FluentAssertions;
using HomeInventory.Domain.Extensions;
using HomeInventory.Domain.Primitives;
using HomeInventory.Tests.Helpers;
using HomeInventory.Tests.Support;

namespace HomeInventory.Tests.Domain;

public class DateTimeServiceExtensionsTests : BaseTest
{
    [Fact]
    public void Today_Should_ReturnDatePart()
    {
        var dateTime = Fixture.Create<DateTimeOffset>();
        var sut = CreateSut(dateTime);

        var result = sut.Today();

        result.Year.Should().Be(dateTime.Year);
        result.Month.Should().Be(dateTime.Month);
        result.Day.Should().Be(dateTime.Day);
    }

    private static IDateTimeService CreateSut(DateTimeOffset dateTime) => new FixedTestingDateTimeService { Now = dateTime };
}
