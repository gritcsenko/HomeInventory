using HomeInventory.Application.UserManagement;
using HomeInventory.Contracts;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;
using HomeInventory.Modules;
using HomeInventory.Modules.Interfaces;

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
        [nameof(AssemblyReferences.DomainPrimitives)] = AssemblyReferences.DomainPrimitives,
        [nameof(AssemblyReferences.Domain)] = AssemblyReferences.Domain,
        ["Contracts"] = new BaseAssemblyReference(typeof(LoginRequest)),
        ["ContractsUserManagement"] = new BaseAssemblyReference(typeof(RegisterRequest)),
        ["ContractsUserManagementValidators"] = new BaseAssemblyReference(typeof(ContractsUserManagementValidatorsModule)),
        ["Modules"] = new BaseAssemblyReference(typeof(ModulesHost)),
        ["ModulesInterfaces"] = new BaseAssemblyReference(typeof(IModule)),
        ["ApplicationUserManagement"] = new BaseAssemblyReference(typeof(ApplicationUserManagementMediatrModule)),
    };

    private static readonly string[] _allowedNamespaces =
    [
        "",
        "System",
        "LanguageExt",
        "FluentValidation",
        "Carter",
        "Serilog",
        "MediatR",
        "Microsoft",
        "AutoMapper",
    ];

    public ArchitectureExpectedDependenciesTheoryData()
    {
        var contracts = _references["Contracts"];
        var contractsUserManagement = _references["ContractsUserManagement"];
        var contractsUserManagementValidators = _references["ContractsUserManagementValidators"];
        var modules = _references["Modules"];
        var modulesInterfaces = _references["ModulesInterfaces"];
        var applicationUserManagement = _references["ApplicationUserManagement"];

        Add(AssemblyReferences.Core, []);
        Add(contractsUserManagement, []);
        Add(contractsUserManagementValidators,
        [
            contractsUserManagement,
            AssemblyReferences.WebFramework,
        ]);
        Add(contracts, []);
        Add(AssemblyReferences.ContractValidations,
        [
            contracts,
            AssemblyReferences.WebFramework,
        ]);
        Add(AssemblyReferences.DomainPrimitives,
        [
            AssemblyReferences.Core,
        ]);
        Add(AssemblyReferences.Domain,
        [
            AssemblyReferences.DomainPrimitives,
            AssemblyReferences.Core,
            modules,
            modulesInterfaces,
        ]);
        Add(AssemblyReferences.Application,
        [
            AssemblyReferences.Domain,
            AssemblyReferences.DomainPrimitives,
            AssemblyReferences.Core,
            modules,
            modulesInterfaces,
        ]);
        Add(AssemblyReferences.WebUserManagement,
        [
            AssemblyReferences.Application,
            AssemblyReferences.ContractValidations, contracts,
            AssemblyReferences.WebFramework,
            AssemblyReferences.Domain,
            AssemblyReferences.DomainPrimitives,
            AssemblyReferences.Core,
            contractsUserManagement,
            modules,
            modulesInterfaces,
        ]);
        Add(AssemblyReferences.Infrastructure,
        [
            AssemblyReferences.Application,
            AssemblyReferences.Domain,
            AssemblyReferences.DomainPrimitives,
            AssemblyReferences.Core,
            modules,
            modulesInterfaces,
        ]);

        Add(AssemblyReferences.Api,
        [
            AssemblyReferences.Application,
            AssemblyReferences.Web,
            AssemblyReferences.Infrastructure,
            AssemblyReferences.WebUserManagement,
            applicationUserManagement,
            AssemblyReferences.WebFramework,
            contractsUserManagementValidators,
            AssemblyReferences.ContractValidations,
            AssemblyReferences.Domain,
            AssemblyReferences.DomainPrimitives,
            AssemblyReferences.Core,
            modules,
            modulesInterfaces,
        ]);
    }

    public static IReadOnlyDictionary<string, IAssemblyReference> References => _references;

    private void Add(IAssemblyReference reference, IAssemblyReference[] allowed)
    {
        var explicitNamespaces = allowed.Concat(reference).SelectMany(r => Extend(r.Namespace));
        var implicitNamespaces = _allowedNamespaces.SelectMany(Extend);
        var namespaces = explicitNamespaces.Concat(implicitNamespaces).ToArray();

        var name = _references.First(p => p.Value == reference).Key;

        Add(name, namespaces);

        static IEnumerable<string> Extend(string ns) => [ns, ns + ".*", ns + ".*.*", ns + ".*.*.*", ns + ".*.*.*.*"];
    }
}
