using HomeInventory.Application.Framework;
using Microsoft.FeatureManagement;

namespace HomeInventory.Tests.Application;

[UnitTest]
public sealed class FeatureFlagTests() : BaseTest<FeatureFlagTests.GivenTestContext>(t => new(t))
{
    private static readonly Variable<IFeatureManager> _manager = new(nameof(_manager));

    [Fact]
    public void ConstructorShouldPreserveName()
    {
        Given
            .New<string>(out var name)
            .Sut(out var sut, name);

        var then = When
            .Invoked(sut, sut => sut.Name);

        then
            .Result(name, (actual, expected) => actual.Should().Be(expected));
    }

    [Fact]
    public void CreateShouldPreserveName()
    {
        Given
            .New<string>(out var name);

        var then = When
            .Invoked(name, FeatureFlag.Create);

        then
            .Result(flag => flag.Should().NotBeNull())
            .Result(name, (actual, expected) => actual.Name.Should().Be(expected));
    }

    [Fact]
    public void CreateWithContextShouldPreserveName()
    {
        Given
            .New<string>(out var name)
            .New<Guid>(out var context);

        var then = When
            .Invoked(name, context, FeatureFlag.Create);

        then
            .Result(flag => flag.Should().NotBeNull())
            .Result(name, (actual, expected) => actual.Name.Should().Be(expected))
            .Result(context, (actual, expected) => actual.Context.Should().Be(expected));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IsEnabledShouldReturnManagerValue(bool expectedValue)
    {
        Given
            .New<string>(out var name)
            .SubstituteFor(out IVariable<IFeatureManager> manager, name,
                (manager, name) => manager.IsEnabledAsync(name).Returns(expectedValue))
            .Sut(out var sut, name);

        var then = await When
            .InvokedAsync(sut, manager, (sut, manager, _) => sut.IsEnabledAsync(manager));

        then
            .Result(flag => flag.Should().Be(expectedValue));
    }

    [Fact]
    public void WithContextShouldReturn()
    {
        Given
            .New<string>(out var name)
            .New<Guid>(out var context)
            .Sut(out var sut, name);

        var then = When
            .Invoked(sut, context, (sut, context) => sut.WithContext(context));

        then
            .Result(flag => flag.Should().NotBeNull())
            .Result(name, (actual, expected) => actual.Name.Should().Be(expected))
            .Result(context, (actual, expected) => actual.Context.Should().Be(expected));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IsEnabledContextShouldReturnManagerValue(bool expectedValue)
    {
        Given
            .New<string>(out var name)
            .New<Guid>(out var context)
            .SubstituteFor(out IVariable<IFeatureManager> manager, name, context,
                (manager, name, context) => manager.IsEnabledAsync(name, context).Returns(expectedValue))
            .Sut(out var sutContext, name, context);

        var then = await When
            .InvokedAsync(sutContext, manager, (sut, manager, _) => sut.IsEnabledAsync(manager));

        then
            .Result(flag => flag.Should().Be(expectedValue));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenTestContext(BaseTest test) : GivenContext<GivenTestContext>(test)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        private static readonly Variable<IFeatureFlag> _sut = new(nameof(_sut));
        private static readonly Variable<IFeatureFlag<Guid>> _sutContext = new(nameof(_sutContext));

        internal GivenTestContext Sut(out IVariable<IFeatureFlag> sutVariable, IVariable<string> nameVariable)
        {
            sutVariable = _sut;
            return Add(_sut, () => Create(nameVariable));
        }

        internal GivenTestContext Sut(out IVariable<IFeatureFlag<Guid>> sutVariable, IVariable<string> nameVariable, IVariable<Guid> contextVariable)
        {
            sutVariable = _sutContext;
            return Add(_sutContext, () => Create(nameVariable, contextVariable));
        }

        private IFeatureFlag Create(IVariable<string> nameVariable) =>
            FeatureFlag.Create(GetValue(nameVariable));

        private IFeatureFlag<TContext> Create<TContext>(IVariable<string> nameVariable, IVariable<TContext> contextVariable)
            where TContext : notnull =>
            FeatureFlag.Create(GetValue(nameVariable), GetValue(contextVariable));
    }
}
