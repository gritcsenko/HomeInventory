using Serilog;

namespace HomeInventory.Api;

internal class AppBuilder(string[] args)
{
    private readonly string[] _args = args;

    public AppBuilder()
        : this(Environment.GetCommandLineArgs())
    {
    }

    public WebApplication Build()
    {
        var builder = WebApplication.CreateBuilder(_args);
        builder.WebHost.CaptureStartupErrors(false);

        AddServices(builder.Services)
            .AddSerilog(builder.Configuration);

        var app = builder.Build();
        app.UseSerilogRequestLogging(static options => options.IncludeQueryInRequestPath = true);
        return app.UseWeb();
    }

    private static IServiceCollection AddServices(IServiceCollection services) =>
        services
            .AddDomain()
            .AddInfrastructure()
            .AddApplication()
            .AddWeb(
                Web.AssemblyReference.Assembly,
                Web.UserManagement.AssemblyReference.Assembly,
                Contracts.Validations.AssemblyReference.Assembly,
                Contracts.UserManagement.Validators.AssemblyReference.Assembly)
            .AddUserManagementInfrastructure();
}
