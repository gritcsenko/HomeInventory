using System.Globalization;
using HomeInventory.Api;
using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;
using Serilog;

using var log = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(SerilogConfigurator.Configure);

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
    log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
}

public partial class Program
{
    protected Program()
    {
    }
}
