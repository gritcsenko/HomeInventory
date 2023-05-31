using HomeInventory.Api;
using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;
using Serilog;

using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args,
    });

    builder.Host.UseSerilog(SerilogConfigurator.Configure, preserveStaticLogger: false, writeToProviders: false);

    builder.Services
        .AddMediatR(MediatRConfigurator.Configure)
        .AddDomain()
        .AddInfrastructure()
        .AddApplication()
        .AddWeb();

    await builder
        .Build()
        .UseWeb()
        .RunAsync();
}
catch (Exception ex)
{
    log.Write(Serilog.Events.LogEventLevel.Fatal, ex, "An unhandled exception occurred during bootstrapping");
}

public partial class Program { }
