using HomeInventory.Modules;

namespace HomeInventory.Api;

internal class AppBuilder(string[] args)
{
    private readonly ModulesCollection _modules = new ApplicationModules();

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
            .AddInfrastructure()
            .AddApplication()
            .AddUserManagementInfrastructure();
}
