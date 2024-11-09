using HomeInventory.Modules;
using HomeInventory.Modules.Interfaces;
using Microsoft.FeatureManagement;

namespace HomeInventory.Tests.Application;

[UnitTest]
public sealed class FeatureFlagTests() : BaseTest<FeatureFlagTests.GivenTestContext>(t => new(t))
{
    [Fact]
    public void ConstructorShouldPreserveName()
    {
        Given
            .New<string>(out var nameVar)
            .Sut(out var sutVar, nameVar);

        var then = When
            .Invoked(sutVar, sut => sut.Name);

        then
            .Result(nameVar, (actual, expected) => actual.Should().Be(expected));
    }

    [Fact]
    public void CreateShouldPreserveName()
    {
        Given
            .New<string>(out var nameVar);

        var then = When
            .Invoked(nameVar, FeatureFlag.Create);

        then
            .Result(flag => flag.Should().NotBeNull())
            .Result(nameVar, (actual, expected) => actual.Name.Should().Be(expected));
    }

    [Fact]
    public void CreateWithContextShouldPreserveName()
    {
        Given
            .New<string>(out var nameVar)
            .New<Guid>(out var contextVar);

        var then = When
            .Invoked(nameVar, contextVar, FeatureFlag.Create);

        then
            .Result(flag => flag.Should().NotBeNull())
            .Result(nameVar, (actual, expected) => actual.Name.Should().Be(expected))
            .Result(contextVar, (actual, expected) => actual.Context.Should().Be(expected));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IsEnabledShouldReturnManagerValue(bool expectedValue)
    {
        Given
            .New<string>(out var nameVar)
            .SubstituteFor(out IVariable<IFeatureManager> managerVar, nameVar,
                (manager, name) => manager.IsEnabledAsync(name).Returns(expectedValue))
            .Sut(out var sutVar, nameVar);

        var then = await When
            .InvokedAsync(sutVar, managerVar, (sut, manager, _) => sut.IsEnabledAsync(manager));

        then
            .Result(flag => flag.Should().Be(expectedValue));
    }

    [Fact]
    public void WithContextShouldReturn()
    {
        Given
            .New<string>(out var nameVar)
            .New<Guid>(out var contextVar)
            .Sut(out var sutVar, nameVar);

        var then = When
            .Invoked(sutVar, contextVar, (sut, context) => sut.WithContext(context));

        then
            .Result(flag => flag.Should().NotBeNull())
            .Result(nameVar, (actual, expected) => actual.Name.Should().Be(expected))
            .Result(contextVar, (actual, expected) => actual.Context.Should().Be(expected));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IsEnabledContextShouldReturnManagerValue(bool expectedValue)
    {
        Given
            .New<string>(out var nameVar)
            .New<Guid>(out var contextVar)
            .SubstituteFor(out IVariable<IFeatureManager> managerVar, nameVar, contextVar,
                (manager, name, context) => manager.IsEnabledAsync(name, context).Returns(expectedValue))
            .Sut(out var sutContext, nameVar, contextVar);

        var then = await When
            .InvokedAsync(sutContext, managerVar, (sut, manager, _) => sut.IsEnabledAsync(manager));

        then
            .Result(flag => flag.Should().Be(expectedValue));
    }

#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class GivenTestContext(BaseTest test) : GivenContext<GivenTestContext>(test)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        internal GivenTestContext Sut(out IVariable<IFeatureFlag> sut, IVariable<string> nameVariable) =>
            New(out sut, () => Create(nameVariable));

        internal GivenTestContext Sut(out IVariable<IFeatureFlag<Guid>> sut, IVariable<string> nameVariable, IVariable<Guid> contextVariable) =>
            New(out sut, () => Create(nameVariable, contextVariable));

        private IFeatureFlag Create(IVariable<string> nameVariable) =>
            FeatureFlag.Create(GetValue(nameVariable));

        private IFeatureFlag<TContext> Create<TContext>(IVariable<string> nameVariable, IVariable<TContext> contextVariable)
            where TContext : notnull =>
            FeatureFlag.Create(GetValue(nameVariable), GetValue(contextVariable));
    }
}
