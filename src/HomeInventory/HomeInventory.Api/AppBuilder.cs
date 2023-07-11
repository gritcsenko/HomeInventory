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
    private readonly WebApplicationBuilder _builder;

    public AppBuilder()
        : this(Environment.GetCommandLineArgs())
    {
    }

    public AppBuilder(string[] args) =>
        _builder = WebApplication.CreateBuilder(args);

    public WebApplication Build()
    {
        _builder.WebHost.CaptureStartupErrors(false);

        AddServices(_builder.Services, _builder.Configuration);

        var app = _builder.Build();
        app.UseSerilogRequestLogging(options => options.IncludeQueryInRequestPath = true);
        return app.UseWeb();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSerilog(configuration)
            .AddMediatR(
                Application.AssemblyReference.Assembly,
                Application.UserManagement.AssemblyReference.Assembly)
            .AddDomain()
            .AddInfrastructure()
            .AddApplication()
            .AddWeb(
                Web.UserManagement.AssemblyReference.Assembly,
                Contracts.UserManagement.Validators.AssemblyReference.Assembly)
            .AddUserManagementWeb()
            .AddUserManagementInfrastructure();
}
