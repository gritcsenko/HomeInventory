using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;

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
        return app.UseWeb();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSerilog(configuration)
            .AddMediatR()
            .AddDomain()
            .AddInfrastructure()
            .AddApplication()
            .AddWeb();
}
