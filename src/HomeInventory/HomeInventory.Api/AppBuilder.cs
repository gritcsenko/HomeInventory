using HomeInventory.Modules;
using Wolverine;

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
        builder.Host.UseWolverine();

        var modulesHost = ModulesHost.Create(ApplicationModules.Instance);

        var modules = await modulesHost.AddServicesAsync(builder.Services, builder.Configuration, builder.Metrics, cancellationToken);

        var app = builder.Build();

        await modules.BuildApplicationAsync(app, cancellationToken);

        app.UseHttpsRedirection();

        return app;
    }
}
