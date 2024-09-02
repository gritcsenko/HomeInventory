using HomeInventory.Api;
using HomeInventory.Contracts;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;
using HomeInventory.Contracts.Validations;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Web.Framework;
using NetArchTest.Rules;

namespace HomeInventory.Tests.Architecture;

public sealed class ArchitectureExpectedDependenciesTheoryData : TheoryData<IAssemblyReference, IAssemblyReference[]>
{
    public ArchitectureExpectedDependenciesTheoryData()
    {
        var domainPrimitives = new BaseAssemblyReference(typeof(ValueObject<>));
        var domain = new BaseAssemblyReference(typeof(DomainModule));
        var contracts = new BaseAssemblyReference(typeof(LoginRequest));
        var contractsUserManagement = new BaseAssemblyReference(typeof(RegisterRequest));
        var contractsUserManagementValidators = new BaseAssemblyReference(typeof(ContractsUserManagementValidatorsModule));

        Add(AssemblyReferences.Core, []);
        Add(contractsUserManagement, []);
        Add(contractsUserManagementValidators, [contractsUserManagement]);
        Add(contracts, []);
        Add(AssemblyReferences.ContractValidations, [contracts]);
        Add(domainPrimitives, [AssemblyReferences.Core]);
        Add(domain, [domainPrimitives, AssemblyReferences.Core]);
        Add(AssemblyReferences.Application, [domain, domainPrimitives, AssemblyReferences.Core]);
        Add(AssemblyReferences.WebUserManagement, [AssemblyReferences.Application, AssemblyReferences.ContractValidations, AssemblyReferences.WebFramework, domain, domainPrimitives, AssemblyReferences.Core, contractsUserManagement]);
        Add(AssemblyReferences.Infrastructure, [AssemblyReferences.Application]);
        Add(AssemblyReferences.Api, [AssemblyReferences.Web, AssemblyReferences.Infrastructure]);
    }
}

public static class AssemblyReferences
{
    public static IAssemblyReference Core { get; } = new BaseAssemblyReference(typeof(BaseAssemblyReference));
    public static IAssemblyReference Application { get; } = new HomeInventory.Application.AssemblyReference();
    public static IAssemblyReference WebUserManagement { get; } = new Web.UserManagement.AssemblyReference();
    public static IAssemblyReference ContractValidations { get; } = new BaseAssemblyReference(typeof(ContractsValidationsModule));
    public static IAssemblyReference Infrastructure { get; } = new Infrastructure.AssemblyReference();
    public static IAssemblyReference Api { get; } = new BaseAssemblyReference(typeof(LoggingModule));
    public static IAssemblyReference Web { get; } = new Web.AssemblyReference();
    public static IAssemblyReference WebFramework { get; } = new BaseAssemblyReference(typeof(ApiModule));
}




[ArchitectureTest]
public class ArchitectureTests
{
    private static readonly string[] _allowedNamespaces = [
        "",
        "System",
        "LanguageExt",
        "FluentValidation",
        "Carter",
        "Serilog",
        "MediatR",
        "HomeInventory.Modules.Interfaces",
        "Microsoft",
        "AutoMapper",
    ];

    [Theory]
    [ClassData<ArchitectureExpectedDependenciesTheoryData>]
    public void Should_NotHaveBadDependencies(IAssemblyReference assemblyReference, IEnumerable<IAssemblyReference> allowed)
    {
        var assembly = assemblyReference.Assembly;
        var namespaces = allowed.Concat(assemblyReference).SelectMany(r => Extend(r.Namespace)).Concat(_allowedNamespaces.SelectMany(Extend)).ToArray();
        var types = Types.InAssembly(assembly);
        var conditions = types.Should();
        var conditionList = conditions.OnlyHaveDependenciesOn(namespaces);
        var result = conditionList.GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();

        static IEnumerable<string> Extend(string ns) => [ns, ns + ".*", ns + ".*.*", ns + ".*.*.*", ns + ".*.*.*.*"];
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


