using System.Globalization;
using HomeInventory.Api;
using HomeInventory.Application;
using HomeInventory.Core;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;
using Serilog;

using var log = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture)
    .CreateLogger();

await Execute.AndCatchAsync(
    async () =>
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
    },
    (Exception ex) => log.Fatal(ex, "An unhandled exception occurred during bootstrapping"));

public partial class Program
{
    protected Program()
    {
    }
}
