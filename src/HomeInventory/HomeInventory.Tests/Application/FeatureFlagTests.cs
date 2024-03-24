using HomeInventory.Application.Framework;
using Microsoft.FeatureManagement;

namespace HomeInventory.Tests.Application;

[UnitTest]
public sealed class FeatureFlagTests() : BaseTest<FeatureFlagTests.GivenTestContext>(t => new(t))
{
    private static readonly Variable<IFeatureFlag> _sut = new(nameof(_sut));
    private static readonly Variable<IFeatureFlag<Guid>> _sutContext = new(nameof(_sutContext));
    private static readonly Variable<string> _name = new(nameof(_name));
    private static readonly Variable<IFeatureManager> _manager = new(nameof(_manager));
    private static readonly Variable<Guid> _context = new(nameof(_context));

    [Fact]
    public void ConstructorShouldPreserveName()
    {
        Given
            .New(_name)
            .Sut(_sut, _name);

        var then = When
            .Invoked(_sut, sut => sut.Name);

        then
            .Result(_name, (actual, expected) => actual.Should().Be(expected));
    }

    [Fact]
    public void CreateShouldPreserveName()
    {
        Given
            .New(_name);

        var then = When
            .Invoked(_name, FeatureFlag.Create);

        then
            .Result(flag => flag.Should().NotBeNull())
            .Result(_name, (actual, expected) => actual.Name.Should().Be(expected));
    }

    [Fact]
    public void CreateWithContextShouldPreserveName()
    {
        Given
            .New(_name)
            .New(_context);

        var then = When
            .Invoked(_name, _context, FeatureFlag.Create);

        then
            .Result(flag => flag.Should().NotBeNull())
            .Result(_name, (actual, expected) => actual.Name.Should().Be(expected))
            .Result(_context, (actual, expected) => actual.Context.Should().Be(expected));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IsEnabledShouldReturnManagerValue(bool expectedValue)
    {
        Given
            .New(_name)
            .SubstituteFor(_manager, _name,
                (manager, name) => manager.IsEnabledAsync(name).Returns(expectedValue))
            .Sut(_sut, _name);

        var then = await When
            .InvokedAsync(_sut, _manager, (sut, manager, _) => sut.IsEnabledAsync(manager));

        then
            .Result(flag => flag.Should().Be(expectedValue));
    }

    [Fact]
    public void WithContextShouldReturn()
    {
        Given
            .New(_name)
            .New(_context)
            .Sut(_sut, _name);

        var then = When
            .Invoked(_sut, _context, (sut, context) => sut.WithContext(context));

        then
            .Result(flag => flag.Should().NotBeNull())
            .Result(_name, (actual, expected) => actual.Name.Should().Be(expected))
            .Result(_context, (actual, expected) => actual.Context.Should().Be(expected));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IsEnabledContextShouldReturnManagerValue(bool expectedValue)
    {
        Given
            .New(_name)
            .New(_context)
            .SubstituteFor(_manager, _name, _context,
                (manager, name, context) => manager.IsEnabledAsync(name, context).Returns(expectedValue))
            .Sut(_sutContext, _name, _context);

        var then = await When
            .InvokedAsync(_sutContext, _manager, (sut, manager, _) => sut.IsEnabledAsync(manager));

        then
            .Result(flag => flag.Should().Be(expectedValue));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenTestContext(BaseTest test) : GivenContext<GivenTestContext>(test)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        internal GivenTestContext Sut(IVariable<IFeatureFlag> sutVariable, IVariable<string> nameVariable) =>
            Add(sutVariable, () => Create(nameVariable));

        internal GivenTestContext Sut<TContext>(IVariable<IFeatureFlag<TContext>> sutVariable, IVariable<string> nameVariable, IVariable<TContext> contextVariable)
            where TContext : notnull =>
            Add(sutVariable, () => Create(nameVariable, contextVariable));

        private IFeatureFlag Create(IVariable<string> nameVariable) =>
            FeatureFlag.Create(GetValue(nameVariable));

        private IFeatureFlag<TContext> Create<TContext>(IVariable<string> nameVariable, IVariable<TContext> contextVariable)
            where TContext : notnull =>
            FeatureFlag.Create(GetValue(nameVariable), GetValue(contextVariable));
    }
}
