using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Domain;

[UnitTest]
public class EquatableComponentTests : BaseTest<EquatableComponentTests.GivenTestContext>
{
    private static readonly Variable<EquatableComponent<string>> _sut = new(nameof(_sut));
    private static readonly Variable<HashCode> _hash = new(nameof(_hash));
    private static readonly Variable<Ulid> _component = new(nameof(_component));

    [Fact]
    public void GetHashCode_ShoudReturnZero_WhenNoComponents()
    {
        Given
            .Component(_sut)
            .EmptyHashCode(_hash);

        When
            .Invoked(_sut, sut => sut.GetHashCode())
            .Result(_hash, (actual, hash) => actual.Should().Be(hash.ToHashCode()));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void GetHashCode_ShoudReturnCombinedComponentsHash_WhenManyComponents(int count)
    {
        Given
            .New(_component, count)
            .AddToHashCode(_hash, _component, count)
            .Component(_sut, _component, count);

        When
            .Invoked(_sut, sut => sut.GetHashCode())
            .Result(_hash, (actual, hash) => actual.Should().Be(hash.ToHashCode()));
    }

    [Fact]
    public void Equals_ShoudBeEqualToEmpty_WhenNoComponents()
    {
        Given
            .Component(_sut)
            .Component(_sut);

        When
            .Invoked(_sut.WithIndex(0), _sut.WithIndex(1), (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeTrue());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Equals_ShoudNotBeEqualToEmpty_WhenManyComponents(int count)
    {
        Given
            .New(_component, count)
            .Component(_sut, _component, count)
            .Component(_sut);

        When
            .Invoked(_sut.WithIndex(0), _sut.WithIndex(1), (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeFalse());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Equals_ShoudBeEqualToComponentWithSameItems_WhenManyComponents(int count)
    {
        Given
            .New(_component, count)
            .Component(_sut, _component, count)
            .Component(_sut, _component, count);

        When
            .Invoked(_sut.WithIndex(0), _sut.WithIndex(1), (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeTrue());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Equals_ShoudNotBeEqualToComponentWithDifferentItems_WhenManyComponents(int count)
    {
        Given
            .New(_component, count * 2)
            .Component(_sut, _component, count)
            .Component(_sut, _component, skip: count, count);

        When
            .Invoked(_sut.WithIndex(0), _sut.WithIndex(1), (sut, other) => sut.Equals(other))
            .Result(actual => actual.Should().BeFalse());
    }

    protected override GivenTestContext CreateGiven(VariablesContainer variables) =>
        new(variables, Fixture);

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenTestContext : GivenContext<GivenTestContext>
#pragma warning restore CA1034 // Nested types should not be visible
    {
        public GivenTestContext(VariablesContainer variables, IFixture fixture)
            : base(variables, fixture)
        {
        }

        public GivenTestContext Component(IVariable<EquatableComponent<string>> sut) =>
            Add(sut, () => new EquatableComponent<string>());

        public GivenTestContext Component<T>(IVariable<EquatableComponent<string>> sut, IVariable<T> variable, int count)
            where T : notnull =>
            Add(sut, () => new EquatableComponent<string>(Variables.Get(variable, count).Cast<object>().ToArray()));

        public GivenTestContext Component<T>(IVariable<EquatableComponent<string>> sut, IVariable<T> variable, int skip, int count)
            where T : notnull =>
            Add(sut, () => new EquatableComponent<string>(Variables.Get(variable, skip + count).Skip(skip).Cast<object>().ToArray()));
    }
}
