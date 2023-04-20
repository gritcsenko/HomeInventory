using HomeInventory.Application;
using HomeInventory.Domain;
using HomeInventory.Infrastructure;
using HomeInventory.Web;
using MediatR.NotificationPublishers;
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

    builder.Host.UseSerilog(ConfigureSerilog, preserveStaticLogger: false, writeToProviders: false);

    builder.Services
        .AddMediatR(ConfigureMediatR)
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

static void ConfigureSerilog(HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);

static void ConfigureMediatR(MediatRServiceConfiguration configuration) =>
    configuration
        .RegisterServicesFromAssemblies(HomeInventory.Application.AssemblyReference.Assembly, HomeInventory.Infrastructure.AssemblyReference.Assembly)
        .SetNotificationPublisher<TaskWhenAllPublisher>()
        .AddLoggingBehavior();

public partial class Program { }
