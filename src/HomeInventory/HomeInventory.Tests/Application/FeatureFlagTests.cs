using HomeInventory.Application.Framework;
using Microsoft.FeatureManagement;

namespace HomeInventory.Tests.Application;

[UnitTest]
public sealed class FeatureFlagTests() : BaseTest<FeatureFlagGivenTestContext>(static t => new(t))
{
    private static readonly Variable<IFeatureManager> _manager = new(nameof(_manager));

    [Fact]
    public void ConstructorShouldPreserveName()
    {
        Given
            .New<string>(out var name)
            .Sut(out var sut, name);

        var then = When
            .Invoked(sut, static sut => sut.Name);

        then
            .Result(name, static (actual, expected) => actual.Should().Be(expected));
    }

    [Fact]
    public void CreateShouldPreserveName()
    {
        Given
            .New<string>(out var name);

        var then = When
            .Invoked(name, FeatureFlag.Create);

        then
            .Result(static flag => flag.Should().NotBeNull())
            .Result(name, static (actual, expected) => actual.Name.Should().Be(expected));
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
            .Result(static flag => flag.Should().NotBeNull())
            .Result(name, static (actual, expected) => actual.Name.Should().Be(expected))
            .Result(context, static (actual, expected) => actual.Context.Should().Be(expected));
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
            .Invoked(sut, context, static (sut, context) => sut.WithContext(context));

        then
            .Result(static flag => flag.Should().NotBeNull())
            .Result(name, static (actual, expected) => actual.Name.Should().Be(expected))
            .Result(context, static (actual, expected) => actual.Context.Should().Be(expected));
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
}
