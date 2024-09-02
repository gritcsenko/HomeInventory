using Carter;
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

internal class AppBuilder(string[] args)
{
    private readonly ModulesCollection _modules = [
        new DomainModule(),
        new LoggingModule(),
        new ContractsValidationsModule(),
        new ContractsUserManagementValidatorsModule(),
        new WebErrorHandling(),
        new WebAuthenticationModule(),
        new DynamicWebAuthorizationModule(),
        new WebSwaggerModule(),
        new WebMappingModule(),
        new WebUerManagementMappingModule(),
        new WebHealthCheckModule(),
        new WebCarterSupportModule(),
    ];

    private readonly string[] _args = args;

    public AppBuilder()
        : this(Environment.GetCommandLineArgs())
    {
    }

    public WebApplication Build()
    {
        var builder = WebApplication.CreateBuilder(_args);
        builder.WebHost.CaptureStartupErrors(false);
        builder.Services.AddSingleton(_modules);

        AddServices(builder.Services);

        _modules.InjectTo(builder);

        var app = builder.Build();

        _modules.BuildInto(app, app);

        app.UseHttpsRedirection();

        return app;
    }

    private static void AddServices(IServiceCollection services) =>
        services
            .AddMediatR(
                new Application.AssemblyReference(),
                new Application.UserManagement.AssemblyReference())
            .AddInfrastructure()
            .AddApplication()
            .AddUserManagementInfrastructure();
}
