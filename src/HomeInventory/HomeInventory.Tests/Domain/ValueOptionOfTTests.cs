using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class ValueOptionOfTTests : BaseTest
{
    [Fact]
    public void None_ShouldReturnOptionWithoutContent()
    {
        var expected = Fixture.Create<Guid>();

        var actual = ValueOption<Guid>.None();

        actual.Should().NotBeNull();
        actual.When(Fixture.Create<Guid>(), expected).Should().Be(expected);
    }

    [Fact]
    public void Some_ShouldReturnOptionWithContent()
    {
        var expected = Fixture.Create<Guid>();

        var actual = ValueOption<Guid>.Some(expected);

        actual.Should().NotBeNull();
        actual.When(expected, Fixture.Create<Guid>()).Should().Be(expected);
    }

    [Fact]
    public void Some_ShouldReturnOptionWithSpecificContent()
    {
        var expected = Fixture.Create<Guid>();

        var actual = ValueOption<Guid>.Some(expected);

        actual.When(c => c, Fixture.Create<Guid>()).Should().Be(expected);
    }

    [Fact]
    public void When_ShouldReturnFirstArgument_WhenOptionHasContent()
    {
        var expected = Fixture.Create<Guid>();
        var option = ValueOption<Guid>.Some(Fixture.Create<Guid>());

        var actual = option.When(expected, Fixture.Create<Guid>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When_ShouldReturnFirstArgument_WhenOptionHasNoContent()
    {
        var expected = Fixture.Create<Guid>();
        var option = ValueOption<Guid>.None();

        var actual = option.When(Fixture.Create<Guid>(), expected);

        actual.Should().Be(expected);
    }

    [Fact]
    public void When2_ShouldReturnFirstArgument_WhenOptionHasContent()
    {
        var expected = Fixture.Create<Guid>();
        var option = ValueOption<Guid>.Some(Fixture.Create<Guid>());

        var actual = option.When(_ => expected, Fixture.Create<Guid>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When2_ShouldReturnContent_WhenOptionHasContent()
    {
        var expected = Fixture.Create<Guid>();
        var option = ValueOption<Guid>.Some(expected);

        var actual = option.When(c => c, Fixture.Create<Guid>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When2_ShouldReturnFirstArgument_WhenOptionHasNoContent()
    {
        var expected = Fixture.Create<Guid>();
        var option = ValueOption<Guid>.None();

        var actual = option.When(_ => Fixture.Create<Guid>(), expected);

        actual.Should().Be(expected);
    }

    [Fact]
    public void When3_ShouldReturnFirstArgument_WhenOptionHasContent()
    {
        var expected = Fixture.Create<Guid>();
        var option = ValueOption<Guid>.Some(Fixture.Create<Guid>());

        var actual = option.When(_ => expected, () => Fixture.Create<Guid>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When3_ShouldReturnContent_WhenOptionHasContent()
    {
        var expected = Fixture.Create<Guid>();
        var option = ValueOption<Guid>.Some(expected);

        var actual = option.When(c => c, () => Fixture.Create<Guid>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void When3_ShouldReturnFirstArgument_WhenOptionHasNoContent()
    {
        var expected = Fixture.Create<Guid>();
        var option = ValueOption<Guid>.None();

        var actual = option.When(_ => Fixture.Create<Guid>(), () => expected);

        actual.Should().Be(expected);
    }
}
