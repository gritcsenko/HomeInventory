using System.Diagnostics.CodeAnalysis;
using HomeInventory.Application;
using HomeInventory.Application.Framework;
using HomeInventory.Application.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;
using HomeInventory.Contracts.Validations;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.UserManagement;
using HomeInventory.Modules;
using HomeInventory.Modules.Interfaces;
using HomeInventory.Web;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.ErrorHandling;
using HomeInventory.Web.Framework;
using HomeInventory.Web.Mapping;
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
        new WebAuthenticationModule(),
        new DynamicWebAuthorizationModule(),
        new WebSwaggerModule(),
        new WebMappingModule(),
        new WebUserManagementModule(),
        new WebUserManagementMappingModule(),
        new WebHealthCheckModule(),
        new WebCarterSupportModule(),
        new ApplicationMediatrSupportModule(),
        new ApplicationMappingModule(),
        new ApplicationMediatrModule(),
        new ApplicationUserManagementMediatrModule(),
        new InfrastructureMappingModule(),
        new InfrastructureDatabaseModule(),
        new InfrastructurePersistenceHealthCheckModule(),
        new InfrastructureSpecificationModule(),
        new InfrastructureUserManagementMappingModule(),
        new InfrastructureUserManagementDatabaseModule(),
        new InfrastructureUserManagementModule(),
    };
}
