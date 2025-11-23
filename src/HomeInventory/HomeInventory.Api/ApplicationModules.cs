using System.Diagnostics.CodeAnalysis;
using HomeInventory.Application.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;
using HomeInventory.Contracts.Validations;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.UserManagement;
using HomeInventory.Modules;
using HomeInventory.Modules.Interfaces;
using HomeInventory.Web;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.ErrorHandling;
using HomeInventory.Web.Framework;
using HomeInventory.Web.OpenApi;
using HomeInventory.Web.UserManagement;

namespace HomeInventory.Api;

internal static class ApplicationModules
{
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Need to use specialized collection")]
    public static IReadOnlyCollection<IModule> Instance { get; } = new ModulesCollection
    {
        new DomainModule(),
        new LoggingModule(),
        new ContractsValidationsModule(),
        new ContractsUserManagementValidatorsModule(),
        new WebErrorHandlingModule(),
        new DynamicWebAuthorizationModule(),
        new WebScalarModule(),
        new WebUserManagementModule(),
        new WebHealthCheckModule(),
        new WebModule(),
        new WebCarterSupportModule(),
        new ApplicationUserManagementModule(),
        new InfrastructureDatabaseModule(),
        new InfrastructurePersistenceHealthCheckModule(),
        new InfrastructureSpecificationModule(),
        new InfrastructureUserManagementDatabaseModule(),
        new InfrastructureUserManagementModule(),
    };
}
