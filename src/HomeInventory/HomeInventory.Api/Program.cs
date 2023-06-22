using HomeInventory.Api;
using HomeInventory.Application;
using HomeInventory.Core;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;
using Serilog;

using var log = SerilogConfigurator.CreateBootstrapLogger();

await Execute.AndCatchAsync(
    async () =>
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddSerilog(builder.Configuration)
            .AddMediatR()
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
