using HomeInventory.Api;
using HomeInventory.Application;
using HomeInventory.Contracts;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;
using HomeInventory.Contracts.Validations;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Web;
using HomeInventory.Web.Framework;
using HomeInventory.Web.UserManagement;
using NetArchTest.Rules;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Architecture;

public sealed class ArchitectureExpectedDependenciesTheoryData : TheoryData<string, string[]>
{
    private static readonly Dictionary<string, IAssemblyReference> _references = new()
    {
        [nameof(AssemblyReferences.Core)] = AssemblyReferences.Core,
        [nameof(AssemblyReferences.Application)] = AssemblyReferences.Application,
        [nameof(AssemblyReferences.WebUserManagement)] = AssemblyReferences.WebUserManagement,
        [nameof(AssemblyReferences.ContractValidations)] = AssemblyReferences.ContractValidations,
        [nameof(AssemblyReferences.Infrastructure)] = AssemblyReferences.Infrastructure,
        [nameof(AssemblyReferences.Api)] = AssemblyReferences.Api,
        [nameof(AssemblyReferences.Web)] = AssemblyReferences.Web,
        [nameof(AssemblyReferences.WebFramework)] = AssemblyReferences.WebFramework,
        ["DomainPrimitives"] = new BaseAssemblyReference(typeof(ValueObject<>)),
        ["Domain"] = new BaseAssemblyReference(typeof(DomainModule)),
        ["Contracts"] = new BaseAssemblyReference(typeof(LoginRequest)),
        ["ContractsUserManagement"] = new BaseAssemblyReference(typeof(RegisterRequest)),
        ["ContractsUserManagementValidators"] = new BaseAssemblyReference(typeof(ContractsUserManagementValidatorsModule)),
    };

    private static readonly string[] _allowedNamespaces = [
      "",
        "System",
        "LanguageExt",
        "FluentValidation",
        "Carter",
        "Serilog",
        "MediatR",
        "HomeInventory.Modules",
        "Microsoft",
        "AutoMapper",
    ];

    public ArchitectureExpectedDependenciesTheoryData()
    {
        var domainPrimitives = _references["DomainPrimitives"];
        var domain = _references["Domain"];
        var contracts = _references["Contracts"];
        var contractsUserManagement = _references["ContractsUserManagement"];
        var contractsUserManagementValidators = _references["ContractsUserManagementValidators"];

        Add(AssemblyReferences.Core);
        Add(contractsUserManagement);
        Add(contractsUserManagementValidators, contractsUserManagement, AssemblyReferences.WebFramework);
        Add(contracts);
        Add(AssemblyReferences.ContractValidations, contracts, AssemblyReferences.WebFramework);
        Add(domainPrimitives, AssemblyReferences.Core);
        Add(domain, domainPrimitives, AssemblyReferences.Core);
        Add(AssemblyReferences.Application, domain, domainPrimitives, AssemblyReferences.Core);
        Add(AssemblyReferences.WebUserManagement, AssemblyReferences.Application, AssemblyReferences.ContractValidations, contracts, AssemblyReferences.WebFramework, domain, domainPrimitives, AssemblyReferences.Core, contractsUserManagement);
        Add(AssemblyReferences.Infrastructure, AssemblyReferences.Application, domain, domainPrimitives, AssemblyReferences.Core);
        Add(AssemblyReferences.Api, AssemblyReferences.Web, AssemblyReferences.Infrastructure, AssemblyReferences.Application, AssemblyReferences.WebUserManagement, AssemblyReferences.WebFramework, contractsUserManagementValidators, AssemblyReferences.ContractValidations, domain, domainPrimitives, AssemblyReferences.Core);
    }

    private void Add(IAssemblyReference reference, params IAssemblyReference[] allowed) 
    {
        var explicitNamespaces = allowed.Concat(reference).SelectMany(r => Extend(r.Namespace)).ToArray();
        var implicitNamespaces = _allowedNamespaces.SelectMany(Extend).ToArray();
        var namespaces = explicitNamespaces.Concat(implicitNamespaces).ToArray();

        var name = _references.First(p => p.Value == reference).Key;

        Add(name, namespaces);
        
        static IEnumerable<string> Extend(string ns) => [ns, ns + ".*", ns + ".*.*", ns + ".*.*.*", ns + ".*.*.*.*"];
    }

    public static IReadOnlyDictionary<string, IAssemblyReference> References => _references;
}

public static class AssemblyReferences
{
    public static IAssemblyReference Core { get; } = new BaseAssemblyReference(typeof(BaseAssemblyReference));
    public static IAssemblyReference Application { get; } = new BaseAssemblyReference(typeof(ApplicationMediatrSupportModule));
    public static IAssemblyReference WebUserManagement { get; } = new BaseAssemblyReference(typeof(WebUerManagementMappingModule));
    public static IAssemblyReference ContractValidations { get; } = new BaseAssemblyReference(typeof(ContractsValidationsModule));
    public static IAssemblyReference Infrastructure { get; } = new Infrastructure.AssemblyReference();
    public static IAssemblyReference Api { get; } = new BaseAssemblyReference(typeof(LoggingModule));
    public static IAssemblyReference Web { get; } = new BaseAssemblyReference(typeof(WebCarterSupportModule));
    public static IAssemblyReference WebFramework { get; } = new BaseAssemblyReference(typeof(ApiCarterModule));
}

[ArchitectureTest]
public class ArchitectureTests
{
    [Theory]
    [ClassData<ArchitectureExpectedDependenciesTheoryData>]
    public void Should_NotHaveBadDependencies(string name, string[] allowed)
    {
        var assemblyReference = ArchitectureExpectedDependenciesTheoryData.References[name];
        var assembly = assemblyReference.Assembly;
        var types = Types.InAssembly(assembly);
        var conditions = types.Should();
        var conditionList = conditions.OnlyHaveDependenciesOn(allowed);
        var result = conditionList.GetResult();

        var failing = result.FailingTypes?.Where(t => t.GetCustomAttribute<CompilerGeneratedAttribute>() is null && t.FullName?.StartsWith("<>z__", StringComparison.Ordinal) != true).ToArray();
        failing.Should().BeNullOrEmpty();
    }

    [Fact]
    public void CommandHandlers_Should_HaveDependencyOn_Domain()
    {
        var assembly = AssemblyReferences.Application.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("CommandHandler", StringComparison.Ordinal)
            .Should()
            .HaveDependencyOn(Namespaces.Domain)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void QueryHandlers_Should_HaveDependencyOn_Domain()
    {
        var assembly = AssemblyReferences.Application.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("QueryHandler", StringComparison.Ordinal)
            .Should()
            .HaveDependencyOn(Namespaces.Domain)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOn_MediatR()
    {
        var assembly = AssemblyReferences.WebUserManagement.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .And()
            .AreNotAbstract()
            .Should()
            .HaveDependencyOn(Namespaces.MediatR)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOn_Automapper()
    {
        var assembly = AssemblyReferences.WebUserManagement.Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller", StringComparison.Ordinal)
            .And()
            .AreNotAbstract()
            .Should()
            .HaveDependencyOn(Namespaces.AutoMapper)
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }
}


