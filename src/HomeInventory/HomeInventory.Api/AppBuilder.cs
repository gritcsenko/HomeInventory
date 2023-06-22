using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;

namespace HomeInventory.Api;

internal class AppBuilder
{
    private readonly WebApplicationBuilder _builder;

    public AppBuilder()
    {
        _builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs());
    }

    public WebApplication Build()
    {
        _builder.Services
          .AddSerilog(_builder.Configuration)
          .AddMediatR()
          .AddDomain()
          .AddInfrastructure()
          .AddApplication()
          .AddWeb();

        return _builder
            .Build()
            .UseWeb();
    }
}
