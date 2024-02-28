namespace HomeInventory.Tests.Middlewares;

[UnitTest]
public class CorrelationIdContainerTests : BaseTest
{
    [Fact]
    public void CorrelationId_Should_NotBeEmpty_When_Created()
    {
        var sut = new CorrelationIdContainer();

        var actual = sut.CorrelationId;

        actual.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateNew_Should_UpdateCorrelationId()
    {
        var sut = new CorrelationIdContainer();
        var original = sut.CorrelationId;

        sut.GenerateNew();

        var actual = sut.CorrelationId;
        actual.Should().NotBe(original);
    }

    [Fact]
    public void SetExisting_Should_UpdateCorrelationId()
    {
        var sut = new CorrelationIdContainer();
        var expected = Fixture.Create<string>();

        sut.SetExisting(expected);

        var actual = sut.CorrelationId;
        actual.Should().Be(expected);
    }
}
