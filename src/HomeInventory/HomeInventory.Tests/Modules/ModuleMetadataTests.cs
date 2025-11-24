using HomeInventory.Modules;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Tests.Modules;

[UnitTest]
public sealed class ModuleMetadataTests() : BaseTest<ModuleMetadataTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void Constructor_ShouldPreserveModule()
    {
        Given
            .Module(out var moduleVar);

        var then = When
            .Invoked(moduleVar, static module => new ModuleMetadata(module));

        then
            .Result(moduleVar, static (result, expected) => result.Module.Should().BeSameAs(expected));
    }

    [Fact]
    public void Constructor_ShouldSetModuleType()
    {
        Given
            .Module(out var moduleVar);

        var then = When
            .Invoked(moduleVar, static module => new ModuleMetadata(module));

        then
            .Result(static result => result.ModuleType.Should().Be<SubjectModule>());
    }

    [Fact]
    public void GetDependencies_WithNoDependencies_ReturnsEmpty()
    {
        Given
            .Module(out var moduleVar)
            .Sut(out var sutVar, moduleVar)
            .EmptyContainer(out var containerVar);

        var then = When
            .Invoked(sutVar, containerVar, static (sut, container) => sut.GetDependencies(container).ToList());

        then
            .Result(static result => result.Should().BeEmpty());
    }

    [Fact]
    public void GetDependencies_WithOneDependency_ReturnsSingleDependency()
    {
        Given
            .Module(out var baseModule)
            .DependentModule(out var dependentModule)
            .Sut(out var sutVar, dependentModule)
            .ContainerWith(out var containerVar, baseModule, dependentModule);

        var then = When
            .Invoked(sutVar, containerVar, static (sut, container) => sut.GetDependencies(container).ToList());

        then
            .Result(baseModule, static (result, expectedModule) =>
                result.Should().ContainSingle().Which
                    .Should().BeSome().Which
                    .Module.Should().BeSameAs(expectedModule));
    }

    [Fact]
    public void GetDependencies_WithMissingDependency_ReturnsNone()
    {
        Given
            .DependentModule(out var dependentModuleVar)
            .Sut(out var sutVar, dependentModuleVar)
            .EmptyContainer(out var containerVar);

        var then = When
            .Invoked(sutVar, containerVar, static (sut, container) => sut.GetDependencies(container).ToList());

        then
            .Result(static result => result.Should().ContainSingle().Which.Should().BeNone());
    }

    [Fact]
    public void ToString_WithNoDependencies_ReturnsModuleName()
    {
        Given
            .Module(out var moduleVar)
            .Sut(out var sutVar, moduleVar);

        var then = When
            .Invoked(sutVar, static sut => sut.ToString());

        then
            .Result(static result => result.Should().Be(nameof(SubjectModule)));
    }

    [Fact]
    public void ToString_WithDependencies_ReturnsModuleNameWithDependencies()
    {
        Given
            .DependentModule(out var dependentModuleVar)
            .Sut(out var sutVar, dependentModuleVar);

        var then = When
            .Invoked(sutVar, static sut => sut.ToString());

        then
            .Result(static result => result.Should().Be($"{nameof(SubjectDependentModule)}:{nameof(SubjectModule)}"));
    }
}

public sealed class ModuleMetadataTestsGivenContext(BaseTest test) : GivenContext<ModuleMetadataTestsGivenContext>(test)
{
    public ModuleMetadataTestsGivenContext Module(out IVariable<IModule> baseModule) =>
        New(out baseModule, static () => new SubjectModule());

    public ModuleMetadataTestsGivenContext DependentModule(out IVariable<IModule> dependentModule) =>
        New(out dependentModule, static () =>
        {
            var module = new SubjectDependentModule();
            module.DependsOn<SubjectModule>();
            return module;
        });

    public ModuleMetadataTestsGivenContext Sut(
        out IVariable<ModuleMetadata> sutVar,
        IVariable<IModule> moduleVar) =>
        New(out sutVar, moduleVar, static module => new(module));

    public ModuleMetadataTestsGivenContext EmptyContainer(
        out IVariable<IReadOnlyCollection<ModuleMetadata>> containerVar) =>
        New(out containerVar, static () => []);

    public ModuleMetadataTestsGivenContext ContainerWith(
        out IVariable<IReadOnlyCollection<ModuleMetadata>> containerVar,
        IVariable<IModule> module1Var,
        IVariable<IModule> module2Var) =>
        New(out containerVar, module1Var, module2Var, static (m1, m2) => [new(m1), new(m2)]);
}

