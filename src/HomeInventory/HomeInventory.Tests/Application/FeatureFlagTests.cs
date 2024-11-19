using HomeInventory.Modules;
using HomeInventory.Modules.Interfaces;
using Microsoft.FeatureManagement;

namespace HomeInventory.Tests.Application;

[UnitTest]
public sealed class FeatureFlagTests() : BaseTest<FeatureFlagGivenTestContext>(static t => new(t))
{
    [Fact]
    public void ConstructorShouldPreserveName()
    {
        Given
            .New<string>(out var nameVar)
            .Sut(out var sutVar, nameVar);

        var then = When
            .Invoked(sutVar, static sut => sut.Name);

        then
            .Result(nameVar, static (actual, expected) => actual.Should().Be(expected));
    }

    [Fact]
    public void CreateShouldPreserveName()
    {
        Given
            .New<string>(out var nameVar);

        var then = When
            .Invoked(nameVar, FeatureFlag.Create);

        then
            .Result(static flag => flag.Should().NotBeNull())
            .Result(nameVar, static (actual, expected) => actual.Name.Should().Be(expected));
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
            .Result(static flag => flag.Should().NotBeNull())
            .Result(nameVar, static (actual, expected) => actual.Name.Should().Be(expected))
            .Result(contextVar, static (actual, expected) => actual.Context.Should().Be(expected));
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
            .Invoked(sutVar, contextVar, static (sut, context) => sut.WithContext(context));

        then
            .Result(static flag => flag.Should().NotBeNull())
            .Result(nameVar, static (actual, expected) => actual.Name.Should().Be(expected))
            .Result(contextVar, static (actual, expected) => actual.Context.Should().Be(expected));
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
}
