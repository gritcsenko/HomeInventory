using System.Diagnostics.CodeAnalysis;
using HomeInventory.Modules.Interfaces;
using Microsoft.FeatureManagement;

namespace HomeInventory.Tests.Modules;

public class BaseModuleTests() : BaseTest<FunctionalModuleTestGivenContext<SubjectModule>>(t => BaseModuleTestGivenContext.Create(t, () => new()))
{
    [Fact]
    public void Dependencies_Should_BeEmpty_When_NoDependencies()
    {
        Given
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, static sut => sut.Dependencies);

        then
            .Result(actual => actual.Should().BeEmpty());
    }

    [Fact]
    public void Dependencies_Should_ContainDependency_When_DeclaredDependency()
    {
        Given
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, static sut => sut.DependsOn<SubjectDependentModule>());

        then
            .Ensure(sutVar,
                sut => sut.Dependencies.Should().ContainSingle(t => t == typeof(SubjectDependentModule)));
    }

    [Fact]
    public async Task Flag_Should_BeAlwaysEnabled()
    {
        Given
            .SubstituteFor<IFeatureManager>(out var featureManagerVar, static m => m.IsEnabledAsync(Arg.Any<string>()).Returns(false))
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, featureManagerVar, static (sut, manager, _) => sut.Flag.IsEnabledAsync(manager));

        then
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public async Task FlagWithContext_Should_BeAlwaysEnabled()
    {
        Given
            .SubstituteFor<object>(out var contextVar)    
            .SubstituteFor<IFeatureManager, object>(out var featureManagerVar, contextVar, static (m, ctx) => m.IsEnabledAsync(Arg.Any<string>(), ctx).Returns(false))
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, featureManagerVar, contextVar, static (sut, manager, ctx, _) => sut.Flag.WithContext(ctx).IsEnabledAsync(manager));

        then
            .Result(actual => actual.Should().BeTrue());
    }

    [Fact]
    public void FlagWithContext_Should_HaveCorrectContext()
    {
        Given
            .SubstituteFor<object>(out var contextVar)    
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, contextVar, static (sut, ctx) => sut.Flag.WithContext(ctx));

        then
            .Result(contextVar, (actual, ctx) => actual.Context.Should().BeSameAs(ctx));
    }

    [Fact]
    public async Task AddServicesAsync_Should_HaveNoSideEffects()
    {
        Given
            .SubstituteFor<IModuleServicesContext>(out var contextVar)    
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, ctx, ct) => sut.AddServicesAsync(ctx, ct));

        then
            .Ensure(sutVar, static sut => sut.Dependencies.Should().BeEmpty())
            .Ensure(contextVar, static ctx => _ = ctx.DidNotReceive().Modules)
            .Ensure(contextVar, static ctx => _ = ctx.DidNotReceive().Configuration)
            .Ensure(contextVar, static ctx => _ = ctx.DidNotReceive().Services)
            .Ensure(contextVar, static ctx => _ = ctx.DidNotReceive().FeatureManager);
    }

    [Fact]
    public async Task BuildAppAsync_Should_HaveNoSideEffects()
    {
        Given
            .SubstituteFor<IModuleBuildContext>(out var contextVar)    
            .Sut(out var sutVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, ctx, ct) => sut.BuildAppAsync(ctx, ct));

        then
            .Ensure(sutVar, static sut => sut.Dependencies.Should().BeEmpty())
            .Ensure(contextVar, static ctx => _ = ctx.DidNotReceive().ApplicationBuilder)
            .Ensure(contextVar, static ctx => _ = ctx.DidNotReceive().EndpointRouteBuilder)
            .Ensure(contextVar, static ctx => _ = ctx.DidNotReceiveWithAnyArgs().GetRequiredService<object>());
    }
}

public sealed class SubjectModule : BaseModule
{
    public new void DependsOn<TModule>()
        where TModule : class, IModule =>
        base.DependsOn<TModule>();
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class SubjectDependentModule : BaseModule
{
}
