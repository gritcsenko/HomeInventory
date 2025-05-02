using HomeInventory.Api;
using HomeInventory.Application.Framework;
using HomeInventory.Application.UserManagement;
using HomeInventory.Contracts;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;
using HomeInventory.Contracts.Validations;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Infrastructure;
using HomeInventory.Modules;
using HomeInventory.Modules.Interfaces;
using HomeInventory.Web.Framework;
using HomeInventory.Web.UserManagement;

namespace HomeInventory.Tests.Architecture;

public static class AssemblyReferences
{
    public static IAssemblyReference Core { get; } = new BaseAssemblyReference(typeof(Execute));
    public static IAssemblyReference DomainPrimitives { get; } = new BaseAssemblyReference(typeof(ValueObject<>));
    public static IAssemblyReference Domain { get; } = new BaseAssemblyReference(typeof(DomainModule));
    public static IAssemblyReference Application { get; } = new BaseAssemblyReference(typeof(ApplicationMediatrSupportModule));
    public static IAssemblyReference ApplicationFramework { get; } = new BaseAssemblyReference(typeof(ApplicationFrameworkServiceCollectionExtensions));
    public static IAssemblyReference WebUserManagement { get; } = new BaseAssemblyReference(typeof(WebUserManagementMappingModule));
    public static IAssemblyReference ContractValidations { get; } = new BaseAssemblyReference(typeof(ContractsValidationsModule));
    public static IAssemblyReference Infrastructure { get; } = new BaseAssemblyReference(typeof(InfrastructureMappingModule));
    public static IAssemblyReference Api { get; } = new BaseAssemblyReference(typeof(LoggingModule));
    public static IAssemblyReference Web { get; } = new BaseAssemblyReference(typeof(WebCarterSupportModule));
    public static IAssemblyReference WebFramework { get; } = new BaseAssemblyReference(typeof(ApiCarterModule));
    public static IAssemblyReference Contracts { get; } = new BaseAssemblyReference(typeof(LoginRequest));
    public static IAssemblyReference ContractsUserManagement { get; } = new BaseAssemblyReference(typeof(RegisterRequest));
    public static IAssemblyReference ContractsUserManagementValidators { get; } = new BaseAssemblyReference(typeof(ContractsUserManagementValidatorsModule));
    public static IAssemblyReference Modules { get; } = new BaseAssemblyReference(typeof(ModulesHost));
    public static IAssemblyReference ModulesInterfaces { get; } = new BaseAssemblyReference(typeof(IModule));
    public static IAssemblyReference ApplicationUserManagement { get; } = new BaseAssemblyReference(typeof(ApplicationUserManagementMediatrModule));
    public static IAssemblyReference DomainUserManagement { get; } = new BaseAssemblyReference(typeof(User));

    public static readonly IReadOnlyDictionary<string, IAssemblyReference> References = new Dictionary<string, IAssemblyReference>
    {
        [nameof(Core)] = Core,
        [nameof(Application)] = Application,
        [nameof(ApplicationFramework)] = ApplicationFramework,
        [nameof(WebUserManagement)] = WebUserManagement,
        [nameof(ContractValidations)] = ContractValidations,
        [nameof(Infrastructure)] = Infrastructure,
        [nameof(Api)] = Api,
        [nameof(Web)] = Web,
        [nameof(WebFramework)] = WebFramework,
        [nameof(DomainPrimitives)] = DomainPrimitives,
        [nameof(Domain)] = Domain,
        [nameof(Contracts)] = Contracts,
        [nameof(ContractsUserManagement)] = ContractsUserManagement,
        [nameof(ContractsUserManagementValidators)] = ContractsUserManagementValidators,
        [nameof(Modules)] = Modules,
        [nameof(ModulesInterfaces)] = ModulesInterfaces,
        [nameof(ApplicationUserManagement)] = ApplicationUserManagement,
        [nameof(DomainUserManagement)] = DomainUserManagement,
    };
}
