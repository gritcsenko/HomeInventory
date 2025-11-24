using HomeInventory.Modules;

namespace HomeInventory.Tests.Modules;

[UnitTest]
public sealed class ModulesCollectionTests() : BaseTest<ModulesCollectionTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void Constructor_ShouldCreateEmptyCollection()
    {
        Given
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, static collection => collection.Count);

        then
            .Result(static result => result.Should().Be(0));
    }

    [Fact]
    public void Add_ShouldAddModule()
    {
        Given
            .New<SubjectModule>(out var expectedModuleVar)
            .Sut(out var sutVar);

        var then = When
            .Invoked(sutVar, expectedModuleVar, static (collection, module) =>
            {
                collection.Add(module);
                return collection;
            });

        then
            .Result(expectedModuleVar, static (result, expected) =>
                result.Should().ContainSingle().Which.Should().BeSameAs(expected));
    }

    [Fact]
    public void Add_SameModuleTypeTwice_ShouldContainOnlyOne()
    {
        Given
            .New<SubjectModule>(out var module1Var)
            .New<SubjectModule>(out var module2Var)
            .Sut(out var sutVar, module1Var);

        var then = When
            .Invoked(sutVar, module2Var, static (collection, module2) =>
            {
                collection.Add(module2);
                return collection;
            });

        then
            .Result(static result => result.Should().ContainSingle());
    }

    [Fact]
    public void Add_DifferentModuleTypes_ShouldContainBoth()
    {
        Given
            .New<SubjectModule>(out var expectedModule1Var)
            .New<SubjectDependentModule>(out var expectedModule2Var)
            .Sut(out var sutVar, expectedModule1Var);

        var then = When
            .Invoked(sutVar, expectedModule2Var, static (collection, module2) =>
            {
                collection.Add(module2);
                return collection;
            });

        then
            .Result(expectedModule1Var, expectedModule2Var, static (result, expectedMod1, expectedMod2) =>
                result.Should().BeEquivalentTo(new object[] { expectedMod1, expectedMod2 }));
    }

    [Fact]
    public void Contains_WhenModuleExists_ReturnsTrue()
    {
        Given
            .New<SubjectModule>(out var moduleVar)
            .Sut(out var sutVar, moduleVar);

        var then = When
            .Invoked(sutVar, moduleVar, static (collection, module) => collection.Contains(module));

        then
            .Result(static result => result.Should().BeTrue());
    }

    [Fact]
    public void Contains_SameModuleType_ReturnsTrue()
    {
        Given
            .New<SubjectModule>(out var module1Var)
            .New<SubjectModule>(out var module2Var)
            .Sut(out var sutVar, module1Var);

        var then = When
            .Invoked(sutVar, module2Var, static (collection, module2) => collection.Contains(module2));

        then
            .Result(static result => result.Should().BeTrue());
    }

    [Fact]
    public void Remove_WhenModuleExists_RemovesModule()
    {
        Given
            .New<SubjectModule>(out var moduleVar)
            .Sut(out var sutVar, moduleVar);

        var then = When
            .Invoked(sutVar, moduleVar, static (collection, module) =>
            {
                collection.Remove(module);
                return collection;
            });

        then
            .Result(static result => result.Should().BeEmpty());
    }

    [Fact]
    public void Remove_SameModuleType_RemovesModule()
    {
        Given
            .New<SubjectModule>(out var module1Var)
            .New<SubjectModule>(out var module2Var)
            .Sut(out var sutVar, module1Var);

        var then = When
            .Invoked(sutVar, module2Var, static (collection, module2) =>
            {
                collection.Remove(module2);
                return collection;
            });

        then
            .Result(static result => result.Should().BeEmpty());
    }
}

public sealed class ModulesCollectionTestsGivenContext(BaseTest test) : GivenContext<ModulesCollectionTestsGivenContext>(test)
{
    public ModulesCollectionTestsGivenContext Sut(
        out IVariable<ModulesCollection> sutVar,
        params IEnumerable<IVariable<SubjectModule>> moduleVars) =>
        New(out sutVar, () =>
        {
            var collection = new ModulesCollection();
            moduleVars.Select(variable => Variables.Get(variable[0])).ForEach(value => collection.Add(value.Value));
            return collection;
        });
}

