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
#pragma warning disable CA1031 // Do not catch general exception types
catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
{
    log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
}

public partial class Program
{
    protected Program()
    {
    }
}
