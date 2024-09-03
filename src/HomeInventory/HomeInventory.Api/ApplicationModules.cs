using HomeInventory.Application;
using HomeInventory.Application.UserManagement;
using HomeInventory.Contracts.UserManagement.Validators;
using HomeInventory.Contracts.Validations;
using HomeInventory.Domain;
using HomeInventory.Modules;
using HomeInventory.Web;
using HomeInventory.Web.Authentication;
using HomeInventory.Web.Authorization.Dynamic;
using HomeInventory.Web.ErrorHandling;
using HomeInventory.Web.Mapping;
using HomeInventory.Web.OpenApi;
using HomeInventory.Web.UserManagement;

namespace HomeInventory.Api;

public sealed class ApplicationModules : ModulesCollection
{
    public ApplicationModules()
    {
        Add(new DomainModule());
        Add(new LoggingModule());
        Add(new ContractsValidationsModule());
        Add(new ContractsUserManagementValidatorsModule());
        Add(new WebErrorHandling());
        Add(new WebAuthenticationModule());
        Add(new DynamicWebAuthorizationModule());
        Add(new WebSwaggerModule());
        Add(new WebMappingModule());
        Add(new WebUerManagementMappingModule());
        Add(new WebHealthCheckModule());
        Add(new WebCarterSupportModule());
        Add(new ApplicationMediatrSupportModule());
        Add(new ApplicationMediatrModule());
        Add(new ApplicationUserManagementMediatrModule());
    }
}
