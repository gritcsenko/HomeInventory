using HomeInventory.Modules;

namespace HomeInventory.Api;

internal class AppBuilder(string[] args)
{
    private readonly string[] _args = args;

    public AppBuilder()
        : this(Environment.GetCommandLineArgs())
    {
    }

    public async Task<WebApplication> BuildAsync(CancellationToken cancellationToken = default)
    {
        var builder = WebApplication.CreateBuilder(_args);
        builder.WebHost.CaptureStartupErrors(false);

        var modulesHost = new ModulesHost(ApplicationModules.Instance);

        await modulesHost.AddModulesAsync(builder.Services, builder.Configuration, cancellationToken);

        var app = builder.Build();

        await modulesHost.BuildModulesAsync(app, cancellationToken);

        app.UseHttpsRedirection();

        return app;
    }
}
