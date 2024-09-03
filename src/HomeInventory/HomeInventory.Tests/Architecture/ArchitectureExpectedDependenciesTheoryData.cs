using HomeInventory.Contracts;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;

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


