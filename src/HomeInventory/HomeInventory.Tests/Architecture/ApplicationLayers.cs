using ArchUnitNET.Domain;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Loader;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace HomeInventory.Tests.Architecture;

public static class ApplicationLayers
{
    public static ArchUnitNET.Domain.Architecture Architecture { get; } = new ArchLoader()
        .LoadAssemblies(AssemblyReferences.References.Values.Select(r => r.Assembly).ToArray())
        .Build();

    public static IObjectProvider<IType> Core { get; } =
        CreateLayer("Core Layer", AssemblyReferences.Core);

    public static IObjectProvider<IType> ModulesSdk { get; } =
        CreateLayer("Modules SDK Layer", AssemblyReferences.Modules, AssemblyReferences.ModulesInterfaces);

    public static IObjectProvider<IType> Domain { get; } =
        CreateLayer("Domain Layer", AssemblyReferences.Domain, AssemblyReferences.DomainPrimitives, AssemblyReferences.DomainUserManagement);

    public static IObjectProvider<IType> Application { get; } =
        CreateLayer("Application Layer", AssemblyReferences.Application, AssemblyReferences.ApplicationFramework, AssemblyReferences.ApplicationUserManagement);

    private static GivenTypesConjunctionWithDescription CreateLayer(string name, params IAssemblyReference[] references) =>
        Types()
            .That()
            .ResideInAssembly(references[0].Assembly, references[1..].Select(r => r.Assembly).ToArray())
            .As(name);
}
