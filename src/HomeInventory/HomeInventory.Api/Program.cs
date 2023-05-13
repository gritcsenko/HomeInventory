using HomeInventory.Api;
using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;
using Serilog;

Log.Logger = new LoggerConfiguration()
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
    Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
