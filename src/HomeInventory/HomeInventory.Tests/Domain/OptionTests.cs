using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class OptionTests : BaseTest
{
    [Fact]
    public void None_ShouldReturnOptionWithoutContent()
    {
        var expected = Fixture.Create<string>();

        var actual = Option.None<string>();

        actual.Should().NotBeNull();
        actual.When(Fixture.Create<string>(), expected).Should().Be(expected);
    }

    [Fact]
    public void None_ShouldReturnSameInstanceEveryCall()
    {
        var expected = Option.None<string>();

        var actual = Option.None<string>();

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void Some_ShouldReturnOptionWithContent()
    {
        var expected = Fixture.Create<string>();

        var actual = Option.Some(expected);

        actual.Should().NotBeNull();
        actual.When(expected, Fixture.Create<string>()).Should().Be(expected);
    }

    [Fact]
    public void Some_ShouldThrow_WhenContentIsNull()
    {
        var expected = Fixture.Create<string>();

        Action action = () => Option.Some<string>(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Some_ShouldReturnOptionWithSpecificContent()
    {
        var expected = Fixture.Create<string>();

        var actual = Option.Some(expected);

        actual.When(c => c, Fixture.Create<string>).Should().Be(expected);
    }

    [Fact]
    public void IsNone_ShouldReturnTrue_WhenOptionWithoutContent()
    {
        var option = Option.None<string>();

        Option.IsNone(option).Should().BeTrue();
    }

    [Fact]
    public void IsNone_ShouldReturnFalse_WhenOptionWithContent()
    {
        var option = Option.Some(Fixture.Create<string>());

        Option.IsNone(option).Should().BeFalse();
    }

    [Fact]
    public void IsSome_ShouldReturnFalse_WhenOptionWithoutContent()
    {
        var option = Option.None<string>();

        Option.IsSome(option).Should().BeFalse();
    }

    [Fact]
    public void IsSome_ShouldReturnTrue_WhenOptionWithContent()
    {
        var option = Option.Some(Fixture.Create<string>());

        Option.IsSome(option).Should().BeTrue();
    }

    [Fact]
    public void Unwrap_ShouldReturnNone_WhenOuterOptionWithoutContent()
    {
        var option = Option.None<Option<string>>();

        var actual = Option.Unwrap(option);

        actual.IsNone().Should().BeTrue();
    }

    [Fact]
    public void Unwrap_ShouldReturnOption_WhenOuterOptionWithContent()
    {
        var expected = Option.None<string>();
        var option = Option.Some(expected);

        var actual = Option.Unwrap(option);

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public void ToOption_ShouldReturnNone_WhenContentIsNull()
    {
        var actual = Option.ToOption<string>(null);

        actual.IsNone().Should().BeTrue();
    }

    [Fact]
    public void ToOption_ShouldReturnSome_WhenContentIsNotNull()
    {
        var expected = Fixture.Create<string>();

        var option = Option.ToOption(expected);

        option.IsSome().Should().BeTrue();
        option.When(x => x, Fixture.Create<string>).Should().Be(expected);
    }

    [Fact]
    public void Reduce_ShouldReturnContent_WhenSome()
    {
        var expected = Fixture.Create<string>();
        var option = Option.ToOption(expected);

        var actual = Option.Reduce(option, Fixture.Create<string>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void Reduce_ShouldReturnOrElse_WhenNone()
    {
        var expected = Fixture.Create<string>();
        var option = Option.None<string>();

        var actual = Option.Reduce(option, expected);

        actual.Should().Be(expected);
    }

    [Fact]
    public void ReduceNullable_ShouldReturnContent_WhenSome()
    {
        var expected = Fixture.Create<string>();
        var option = Option.ToOption(expected);

        var actual = Option.ReduceNullable(option, Fixture.Create<string>());

        actual.Should().Be(expected);
    }

    [Fact]
    public void ReduceNullable_ShouldReturnOrElse_WhenNone()
    {
        var expected = Fixture.Create<string>();
        var option = Option.None<string>();

        var actual = Option.ReduceNullable(option, expected);

        actual.Should().Be(expected);
    }

    [Fact]
    public void ReduceNullable_ShouldReturnDefaultForOrElse_WhenNone()
    {
        var option = Option.None<string>();

        var actual = Option.ReduceNullable(option);

        actual.Should().BeNull();
    }

    [Fact]
    public void Reduce2_ShouldReturnContent_WhenSome()
    {
        var expected = Fixture.Create<string>();
        var option = Option.ToOption(expected);

        var actual = Option.Reduce(option, Fixture.Create<string>);

        actual.Should().Be(expected);
    }

    [Fact]
    public void Reduce2_ShouldReturnOrElse_WhenNone()
    {
        var expected = Fixture.Create<string>();
        var option = Option.None<string>();

        var actual = Option.Reduce(option, () => expected);

        actual.Should().Be(expected);
    }

    [Fact]
    public void Select_ShouldReturnNone_WhenNone()
    {
        var expected = Fixture.Create<string>();
        var option = Option.None<string>();

        var actual = Option.Select(option, c => c.Some());

        actual.IsNone().Should().BeTrue();
    }

    [Fact]
    public void Select_ShouldReturnResultFromSelector_WhenSome()
    {
        var expected = Fixture.Create<string>();
        var option = Option.Some(Fixture.Create<string>());

        var actual = Option.Select(option, _ => expected.Some());

        actual.IsSome().Should().BeTrue();
        actual.Reduce(Fixture.Create<string>()).Should().Be(expected);
    }

    [Fact]
    public void Select2_ShouldReturnNone_WhenNone()
    {
        var expected = Fixture.Create<string>();
        var option = Option.None<string>();

        var actual = Option.Select(option, c => c);

        actual.IsNone().Should().BeTrue();
    }

    [Fact]
    public void Select2_ShouldReturnResultFromSelector_WhenSome()
    {
        var expected = Fixture.Create<string>();
        var option = Option.Some(Fixture.Create<string>());

        var actual = Option.Select(option, _ => expected);

        actual.IsSome().Should().BeTrue();
        actual.Reduce(Fixture.Create<string>()).Should().Be(expected);
    }

    [Fact]
    public void Where_ShouldReturnSome_WhenConditionIsTrueAndSome()
    {
        var expected = Fixture.Create<string>();
        var option = Option.Some(expected);

        var actual = Option.Where(option, _ => true);

        actual.IsSome().Should().BeTrue();
        actual.Reduce(Fixture.Create<string>()).Should().Be(expected);
    }

    [Fact]
    public void Where_ShouldReturnNone_WhenConditionIsFalseAndSome()
    {
        var expected = Fixture.Create<string>();
        var option = Option.Some(expected);

        var actual = Option.Where(option, _ => false);

        actual.IsNone().Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Where_ShouldReturnNone_WhenNone(bool condition)
    {
        var option = Option.None<string>();

        var actual = Option.Where(option, _ => condition);

        actual.IsNone().Should().BeTrue();
    }

    [Fact]
    public void WhereNot_ShouldReturnSome_WhenConditionIsFalseAndSome()
    {
        var expected = Fixture.Create<string>();
        var option = Option.Some(expected);

        var actual = Option.WhereNot(option, _ => false);

        actual.IsSome().Should().BeTrue();
        actual.Reduce(Fixture.Create<string>()).Should().Be(expected);
    }

    [Fact]
    public void WhereNot_ShouldReturnNone_WhenConditionIsTrueAndSome()
    {
        var expected = Fixture.Create<string>();
        var option = Option.Some(expected);

        var actual = Option.WhereNot(option, _ => true);

        actual.IsNone().Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WhereNot_ShouldReturnNone_WhenNone(bool condition)
    {
        var option = Option.None<string>();

        var actual = Option.WhereNot(option, _ => condition);

        actual.IsNone().Should().BeTrue();
    }
}
