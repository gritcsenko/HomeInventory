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

    public async Task<WebApplication> BuildAsync()
    {
        var builder = WebApplication.CreateBuilder(_args);
        builder.WebHost.CaptureStartupErrors(false);

        var modulesHost = new ModulesHost(_modules);

        await modulesHost.AddModulesAsync(builder.Services, builder.Configuration);

        var app = builder.Build();

        await modulesHost.BuildModulesAsync(app);

        app.UseHttpsRedirection();

        return app;
    }
}
