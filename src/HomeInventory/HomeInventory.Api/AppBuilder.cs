using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.UserManagement;
using HomeInventory.Web;
using HomeInventory.Web.UserManagement;
using Serilog;

namespace HomeInventory.Api;

internal class AppBuilder
{
    private readonly string[] _args;

    public AppBuilder()
        : this(Environment.GetCommandLineArgs())
    {
    }

    public AppBuilder(string[] args) =>
        _args = args;

    public WebApplication Build()
    {
        var builder = WebApplication.CreateBuilder(_args);
        builder.WebHost.CaptureStartupErrors(false);

        AddServices(builder.Services)
            .AddSerilog(builder.Configuration);

        var app = builder.Build();
        app.UseSerilogRequestLogging(options => options.IncludeQueryInRequestPath = true);
        return app.UseWeb();
    }

    private static IServiceCollection AddServices(IServiceCollection services) =>
        services
            .AddMediatR(
                Application.AssemblyReference.Assembly,
                Application.UserManagement.AssemblyReference.Assembly)
            .AddDomain()
            .AddInfrastructure()
            .AddApplication()
            .AddWeb(
                Web.AssemblyReference.Assembly,
                Web.UserManagement.AssemblyReference.Assembly,
                Contracts.Validations.AssemblyReference.Assembly,
                Contracts.UserManagement.Validators.AssemblyReference.Assembly)
            .AddUserManagementWeb()
            .AddUserManagementInfrastructure();
}
