using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class OptionOfTTests : BaseTest
{
    [Fact]
    public void None_ShouldReturnOptionWithoutContent()
    {
        var expected = Fixture.Create<string>();

        var actual = Option<string>.None();

        actual.Should().NotBeNull();
        actual.When(Fixture.Create<string>(), expected).Should().Be(expected);
    }

    [Fact]
    public void None_ShouldReturnSameInstanceEveryCall()
    {
        var expected = Option<string>.None();

        var actual = Option<string>.None();

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void Some_ShouldReturnOptionWithContent()
    {
        var expected = Fixture.Create<string>();

        var actual = Option<string>.Some(expected);

        actual.Should().NotBeNull();
        actual.When(expected, Fixture.Create<string>()).Should().Be(expected);
    }

    [Fact]
    public void Some_ShouldThrow_WhenContentIsNull()
    {
        var expected = Fixture.Create<string>();

        Action action = () => Option<string>.Some(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Some_ShouldReturnOptionWithSpecificContent()
    {
        var expected = Fixture.Create<string>();

        var actual = Option<string>.Some(expected);

        actual.When(c => c, Fixture.Create<string>()).Should().Be(expected);
    }

    [Fact]
    public void When_ShouldReturnFirstArgument_WhenOptionHasContent()
    {
        var expected = Fixture.Create<string>();
        var option = Option<string>.Some(Fixture.Create<string>());

        var actual = option.When(expected, Fixture.Create<string>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When_ShouldReturnFirstArgument_WhenOptionHasNoContent()
    {
        var expected = Fixture.Create<string>();
        var option = Option<string>.None();

        var actual = option.When(Fixture.Create<string>(), expected);

        actual.Should().Be(expected);
    }

    [Fact]
    public void When2_ShouldReturnFirstArgument_WhenOptionHasContent()
    {
        var expected = Fixture.Create<string>();
        var option = Option<string>.Some(Fixture.Create<string>());

        var actual = option.When(_ => expected, Fixture.Create<string>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When2_ShouldReturnContent_WhenOptionHasContent()
    {
        var expected = Fixture.Create<string>();
        var option = Option<string>.Some(expected);

        var actual = option.When(c => c, Fixture.Create<string>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When2_ShouldReturnFirstArgument_WhenOptionHasNoContent()
    {
        var expected = Fixture.Create<string>();
        var option = Option<string>.None();

        var actual = option.When(_ => Fixture.Create<string>(), expected);

        actual.Should().Be(expected);
    }

    [Fact]
    public void When3_ShouldReturnFirstArgument_WhenOptionHasContent()
    {
        var expected = Fixture.Create<string>();
        var option = Option<string>.Some(Fixture.Create<string>());

        var actual = option.When(_ => expected, () => Fixture.Create<string>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When3_ShouldReturnContent_WhenOptionHasContent()
    {
        var expected = Fixture.Create<string>();
        var option = Option<string>.Some(expected);

        var actual = option.When(c => c, () => Fixture.Create<string>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When3_ShouldReturnFirstArgument_WhenOptionHasNoContent()
    {
        var expected = Fixture.Create<string>();
        var option = Option<string>.None();

        var actual = option.When(_ => Fixture.Create<string>(), () => expected);

        actual.Should().Be(expected);
    }
}
