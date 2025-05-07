using NetArchTest.Rules;
using System.Reflection;
using System.Runtime.CompilerServices;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using HomeInventory.Api;
using HomeInventory.Modules.Interfaces;
using Mono.Cecil;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace HomeInventory.Tests.Architecture;

[ArchitectureTest]
public class ArchitectureTests
{
    [Theory]
    [ClassData<ArchitectureExpectedDependenciesTheoryData>]
    public void Should_NotHaveBadDependencies(string name, string[] allowed)
    {
        var assemblyReference = ArchitectureExpectedDependenciesTheoryData.References[name];
        var assembly = assemblyReference.Assembly;
        var types = NetArchTest.Rules.Types.InAssembly(assembly);
        var conditions = types.Should();
        var conditionList = conditions.OnlyHaveDependenciesOn(allowed);
        var result = conditionList.GetResult();

        var failing = result.FailingTypes?.Where(static t =>
            t.GetCustomAttribute<CompilerGeneratedAttribute>() is null
            && t.FullName?.StartsWith("<>", StringComparison.Ordinal) != true
            && !t.Name.EndsWith("Module", StringComparison.Ordinal)
            && !t.Name.EndsWith("Modules", StringComparison.Ordinal)
            && !t.Name.EndsWith("Mappings", StringComparison.Ordinal)
            && !t.Name.EndsWith("HealthCheck", StringComparison.Ordinal)).ToArray();
        failing.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Layers_Dependencies() =>
        LayerDependencies.GetAllLayers()
            .SelectMany(layer =>
                LayerDependencies.NotDependsOn(layer)
                    .Select(unwanted => Types()
                        .That()
                        .Are(layer)
                        .Should()
                        .NotDependOnAny(unwanted)))
            .Cast<IArchRule>()
            .Aggregate((a, b) => a.And(b))
            .Check(ApplicationLayers.Architecture);

    [Fact]
    public void CommandHandlers_Should_HaveDependencyOn_Domain()
    {
        var assembly = AssemblyReferences.Application.Assembly;

        var result = NetArchTest.Rules.Types.InAssembly(assembly)
            .That().HaveNameEndingWith("CommandHandler", StringComparison.Ordinal)
            .Should().HaveDependencyOn(Namespaces.Domain)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void QueryHandlers_Should_HaveDependencyOn_Domain()
    {
        var assembly = AssemblyReferences.Application.Assembly;

        var result = NetArchTest.Rules.Types.InAssembly(assembly)
            .That().HaveNameEndingWith("QueryHandler", StringComparison.Ordinal)
            .Should().HaveDependencyOn(Namespaces.Domain)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOn_MediatR()
    {
        var assembly = AssemblyReferences.WebUserManagement.Assembly;

        var result = NetArchTest.Rules.Types.InAssembly(assembly)
            .That().HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .And().AreNotAbstract()
            .Should().HaveDependencyOn(Namespaces.MediatR)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOn_Automapper()
    {
        var assembly = AssemblyReferences.WebUserManagement.Assembly;

        var result = NetArchTest.Rules.Types.InAssembly(assembly)
            .That().HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .And().AreNotAbstract()
            .Should().HaveDependencyOn(Namespaces.AutoMapper)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Modules_Should_EndWithModule()
    {
        _ = ApplicationModules.Instance;
        var result = NetArchTest.Rules.Types.InCurrentDomain()
            .That().MeetCustomRule(new RuntimeClassesRule())
            .And().AreClasses()
            .And().AreNotAbstract()
            .And().Inherit(typeof(BaseModule))
            .Should().BeSealed()
            .And().HaveNameEndingWith("Module", StringComparison.Ordinal)
            .GetResult();
        result.FailingTypeNames.Should().BeNullOrEmpty();
    }
}

file sealed class RuntimeClassesRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
    {
        if (type.Namespace is not { } ns)
        {
            return false;
        }

        return ns.StartsWith("HomeInventory", StringComparison.Ordinal)
               && !ns.Contains("Test", StringComparison.Ordinal);
    }
}
